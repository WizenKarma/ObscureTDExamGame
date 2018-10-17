using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genGrid : MonoBehaviour {
    public int numberOfTiles = 81;
    public float tilegap = 1f;
    public int tilesPerRow = 9;
    public GameObject tileprefab;
	//private int tilesCreated;



    // Use this for initialization
    void Start () {
        createTiles();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void createTiles()
    {
        float xOffset = 0.0f;
        float zOffset = 0.0f;

        for(int tilesCreated= 0; tilesCreated<numberOfTiles; tilesCreated += 1)
        {
            xOffset += tilegap;
            if(tilesCreated % tilesPerRow == 0)
            {
                zOffset += tilegap;
                xOffset = 0;
            }
            print("created");
            print(tilesCreated);
           var clone = Instantiate(tileprefab,new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset), Quaternion.identity);
            clone.name = "tile" + tilesCreated;
           
        }

    }
}
