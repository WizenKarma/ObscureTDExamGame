using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPlacer : MonoBehaviour {

    private grid grid;
    public Camera cam;
    public GameObject tilePrefab;
    public int tilesCreated;
    public LayerMask wallLayer;
    public LayerMask floorLayer;

    private void Awake()
    {
        grid = FindObjectOfType<grid>();
    }

    private void Update()
    {
         if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, floorLayer))
            {
                PlaceCubeNear(hitInfo.point);
            }
            tilesCreated += 1;
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hitInfo;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity,wallLayer))
            {
                hitInfo.collider.GetComponent<AdaptiveWall>().killWall();
            }
            tilesCreated += 1;
        }
    }

    public Vector3 PlaceCubeNear(Vector3 clickPoint)
    {
        Vector3 finalPosition = grid.GetNearestPointOnGrid(clickPoint);
        return finalPosition;
    }

    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
	}
	
	
}
