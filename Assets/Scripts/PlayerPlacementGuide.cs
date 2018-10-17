using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacementGuide : MonoBehaviour {
    Transform camTransform;
    float cameraRayLength = 20f;
    public LayerMask groundedLayer;
    private itemPlacer ip;
    public GameObject cursorBlock;
	// Use this for initialization
	void Start () {
        camTransform = Camera.main.transform;
        ip = FindObjectOfType<itemPlacer>();
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitInfo;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hitInfo, cameraRayLength, groundedLayer))
        {
           cursorBlock.transform.position = ip.PlaceCubeNear(hitInfo.point) - Vector3.up*0.45f;
        }
    }
}
