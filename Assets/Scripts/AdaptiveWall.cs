using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveWall : MonoBehaviour {
    [SerializeField]
    LayerMask avoid;
    [SerializeField]
    bool[] around; //forward, right, backward, left
    [SerializeField]
    GameObject corner; 
    [SerializeField]
    GameObject terminate;
    [SerializeField]
    GameObject junction;

    public float rayLength;
    public GameObject cornerHolder;
    enum Direction { up, down, left, right };
	// Use this for initialization
	void Start () {
        behavior(true);
    }

    void behavior(bool sendOut) {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, avoid))
        {
            hit.collider.gameObject.GetComponent<AdaptiveWall>().mutate(Direction.up,sendOut);
            around[0] = sendOut;
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, rayLength, avoid))
        {
            hit.collider.gameObject.GetComponent<AdaptiveWall>().mutate(Direction.down,sendOut);
            around[2] = sendOut;
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, rayLength, avoid))
        {
            hit.collider.gameObject.GetComponent<AdaptiveWall>().mutate(Direction.left,sendOut);
            around[3] = sendOut;
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.left), out hit, rayLength, avoid))
        {
            hit.collider.gameObject.GetComponent<AdaptiveWall>().mutate(Direction.right,sendOut);
            around[1] = sendOut;
        }
        interpret();
    }

    public void killWall() {
        behavior(false);
        this.GetComponent<Collider>().enabled = false;
        
        if (cornerHolder != null)
        {
            Destroy(cornerHolder.gameObject);
            cornerHolder = null;
        }
        Destroy(this.gameObject);
    }

    void mutate(Direction dir, bool value) {
        switch (dir) {
            case Direction.up: around[2] = value;                                  //This indicates that the object has been hit by a raycast from BELOW. The other game object has projected a ray UP
                break;
            case Direction.down: around[0] = value;
                break;
            case Direction.left: around[1] = value;
                break;
            case Direction.right: around[3] = value;
                break;
        }
        interpret();
    }

    void interpret()
    {
        if (cornerHolder != null)
        {
            Destroy(cornerHolder.gameObject);
            cornerHolder = null;
        }
        if ((around[0] || around[1] || around[2] || around[3]))
        {
            for (int i = 0; i<4; i++)
            {
                int count = 0;
                if (!around[i])
                {
                    for (int j = 0; j<4; j++)
                    {
                        if (j != i)
                        {
                            if (around[j])
                                count++;
                        }   
                    }
                    if (count == 3)
                    {
                        this.GetComponent<MeshRenderer>().enabled = false;
                        //this indicates that it should be a T junction
                        switch (i)
                        {
                            case 0:
                                {
                                    cornerHolder = Instantiate(junction, this.transform.position, Quaternion.Euler(0, 90, 0));
                                    break;
                                }
                            case 1:
                                {
                                    cornerHolder = Instantiate(junction, this.transform.position, Quaternion.Euler(0, 180, 0));
                                    break;
                                }
                            case 2:
                                {
                                    cornerHolder = Instantiate(junction, this.transform.position, Quaternion.Euler(0, 270, 0));
                                    break;
                                }
                            case 3:
                                {
                                    cornerHolder = Instantiate(junction, this.transform.position, Quaternion.Euler(0, 0, 0));
                                    break;
                                }
                        }
                        return;   
                    }
                }
            }
            if ((around[0] && around[2]) || (around[1] && around[3]))
            { // this indicates that it should be a square piece, as it has two vertically opposed walls on either side
                if (around[0])
                    this.transform.rotation = Quaternion.Euler(0, 90, 0);

                this.GetComponent<MeshRenderer>().enabled = true;
                return;
            }
            if ((around[1] || around[3]) && (around[0] || around[2]))
            { // this indicates that is should be a corner, as it is not a square and has two walls on adjacent sides
                this.GetComponent<MeshRenderer>().enabled = false;
                if (around[1])
                {
                    if (around[0])
                    {
                        cornerHolder = Instantiate(corner, this.transform.position, Quaternion.Euler(0,270,0));
                        //print("corner should be on the bottom left: " + this.name); // This would suggest that the prefab.forward would be facing up
                    }
                    else
                    {
                        cornerHolder = Instantiate(corner, this.transform.position, Quaternion.Euler(0, 0, 0));
                        //print("corner should be on the top left: " + this.name); // Facing right 90
                    }

                }
                else
                {
                    if (around[0])
                    {
                        cornerHolder = Instantiate(corner, this.transform.position, Quaternion.Euler(0, 180, 0));
                        //print("corner should be on the bottom right: " + this.name); // Facing left 90
                    }
                    else
                    {
                        cornerHolder = Instantiate(corner, this.transform.position, Quaternion.Euler(0, 90, 0));
                        //print("corner should be on the top right: " + this.name); // Facing right/left 180
                    }
                }
                return;
            }
            // this indicates that it should terminate and is touching only 1 other wall
            this.GetComponent<MeshRenderer>().enabled = false;
            if (around[1])
                cornerHolder = Instantiate(terminate, this.transform.position, Quaternion.Euler(0, 0, 0));
            else if (around[2])
                cornerHolder = Instantiate(terminate, this.transform.position, Quaternion.Euler(0, 90, 0));
            else if (around[3])
                cornerHolder = Instantiate(terminate, this.transform.position, Quaternion.Euler(0, 180, 0));
            else if (around[0])
                cornerHolder = Instantiate(terminate, this.transform.position, Quaternion.Euler(0, 270, 0));
            return;
        }
        this.GetComponent<MeshRenderer>().enabled = true;
    }
}
