using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 220f;   
    private Rigidbody myRb;


    #region ROTATION_VARS
    public float mouse_Sensitivity = 50.0f;
    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    private GameObject pivot;
    private Vector3 oneZeroOne;
    #endregion

    #region TOP_DOWN_VARS
    public float snapSpeed = 10f;
    public GameObject posToSnapTo;
    public bool isTopDown;
    private Transform previousTransform;
    private Vector3 previousPos;
    public float snapTime = 10f;
    public GameObject lookAtTarget;
    private Vector3 snapVector = Vector3.zero;
    #endregion


    #region CAMERA_MOVEMENT_ROTATION
    private void CameraRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (!isTopDown)
        {
            mouseX += Input.GetAxisRaw("Mouse X");
            mouseY -= Input.GetAxisRaw("Mouse Y");
            float angle = Mathf.Atan2(mouseY, mouseX) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
            transform.rotation = rotation;
            transform.forward = pivot.transform.forward;
            transform.LookAt(pivot.transform);
            pivot.transform.forward = transform.forward;
        }
    }

    private void CameraMovement()
    {
        #region KILL_MOVEMENT
        if (Input.GetKeyUp(KeyCode.W) |
            Input.GetKeyUp(KeyCode.A) |
            Input.GetKeyUp(KeyCode.D) |
            Input.GetKeyUp(KeyCode.S) |
            Input.GetKeyUp(KeyCode.Space) |
            Input.GetKeyUp(KeyCode.LeftShift))
        {
            myRb.velocity = Vector3.zero;
        }
        #endregion

        #region SPECTATE_MOVEMENT
        if (!isTopDown)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                myRb.velocity = new Vector2(0, 1 * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                myRb.velocity = new Vector2(0, -1 * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.W))
            {
                myRb.velocity = transform.forward * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                myRb.velocity = -transform.right * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                myRb.velocity = transform.right * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                myRb.velocity = -transform.forward * moveSpeed * Time.deltaTime;
            }
        }
        #endregion

        #region TOP_DOWN_MOVEMENT
        if (isTopDown)
        {
            Vector3 input = new Vector3();
            input.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
            input.z = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
            myRb.velocity = input;
        }
        #endregion
    }
    #endregion

    #region TOP_DOWN_FUNCTIONALITY 

    private void ToggleTopDown()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(!isTopDown)
            {
                SnapToTopDown();
            }
            else
            {
                ReturnToSpectate();
            }
        }
    }

    private void SnapToTopDown()
    {
        isTopDown = true;
        previousPos = transform.position;
        previousTransform = transform;
        snapVector = new Vector3(90f, 0, 0);
        transform.position = Vector3.Slerp(transform.position, posToSnapTo.transform.position, snapSpeed);
        transform.eulerAngles = snapVector;
    }


    private void ReturnToSpectate()
    {
        isTopDown = false;
        transform.position = Vector3.Lerp(transform.position, previousPos, snapSpeed);
    }
    #endregion

    #region UNITY_FUNCTIONS
    private void Awake()
    {
        previousTransform = transform;
        oneZeroOne = new Vector3(1, 1, 1);
        myRb = GetComponent<Rigidbody>();
        myRb.useGravity = false;
        pivot = GameObject.Find("pivot");
    }

    void Update()
    {
        CameraRotation();
        CameraMovement();
        ToggleTopDown();
    }
    #endregion
}
