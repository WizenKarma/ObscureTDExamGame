using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    bool isTopDown;
    Rigidbody myRb;
    public float speedMultiplier;
    public float shiftMultiplier;
    public float snapSpeed;
    public float mouseScrollMultiplier;
    public GameObject posToSnapTo;

    Vector3 previousPos;
    Transform previousTransform;
    Vector3 snapVector;

    #region Rotation Variables
    private Camera cam;
    private Transform camTransform;
    private float MIN_Y = -89.0f;
    private float MAX_Y = 89.0f;
    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    private GameObject pivot;
    private bool cursorOn;
    #endregion

    // Use this for initialization
    void Start()
    {
        //Cursor Initialization
        Cursor.visible = false;
        cursorOn = false;
        Cursor.lockState = CursorLockMode.Locked;

        isTopDown = false;
        myRb = this.GetComponent<Rigidbody>();
        pivot = GameObject.Find("pivot");
        cam = Camera.main;
        camTransform = cam.transform;
    }

    private void CameraRotation()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            cursorOn = !cursorOn;
            Cursor.visible = cursorOn;
            if (!cursorOn)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }
        if (!cursorOn)
            if (!isTopDown)
            {
                mouseX += Input.GetAxisRaw("Mouse X");
                mouseY -= Input.GetAxisRaw("Mouse Y");
                float angle = Mathf.Atan2(mouseY, mouseX) * Mathf.Rad2Deg;
                mouseY = Mathf.Clamp(mouseY, MIN_Y, MAX_Y);
                Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
                transform.rotation = rotation;
                transform.forward = pivot.transform.forward;
                transform.LookAt(pivot.transform);
                pivot.transform.forward = transform.forward;
            }
    }

    void viewMode()
    {
        if (!isTopDown && Input.GetKeyDown(KeyCode.T))
        {
            isTopDown = true;
            previousPos = transform.position;
            previousTransform = transform;
            snapVector = new Vector3(90f, 0, 0);
            transform.position = Vector3.Slerp(transform.position, posToSnapTo.transform.position, snapSpeed);
            transform.eulerAngles = snapVector;
        } else if (isTopDown && Input.GetKeyDown(KeyCode.T))
        {
            isTopDown = false;
            transform.position = Vector3.Lerp(transform.position, previousPos, snapSpeed);
        }
        
    }
    void move()
    {
        float moveSpeed = speedMultiplier;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = moveSpeed * shiftMultiplier;
        }
        if (!isTopDown)
        {
            myRb.velocity = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("WorldVertical")) * Time.deltaTime * moveSpeed;
        }
        else
        {
            myRb.velocity = (Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal") + Vector3.down * Input.GetAxis("Mouse ScrollWheel") * mouseScrollMultiplier) * Time.deltaTime * moveSpeed;
        }
    }
    // Update is called once per frame
    void Update()
    {
        CameraRotation();
        viewMode();
        move();
    }
}
