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
    public GameObject posToSnapTo;

    Vector3 previousPos;
    Transform previousTransform;
    Vector3 snapVector;

    
    // Use this for initialization
    void Start()
    {
        isTopDown = false;
        myRb = this.GetComponent<Rigidbody>();
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
            Vector3 input = new Vector3();
            input.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
            input.z = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
            myRb.velocity = (Vector3.forward * Input.GetAxis("Vertical") + Vector3.left * Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed;

        }
    }
    // Update is called once per frame
    void Update()
    {
        viewMode();
        move();
    }
}
