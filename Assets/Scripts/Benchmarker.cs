using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benchmarker : MonoBehaviour {
    public GameObject walls;
	// Use this for initialization
	void Start () {
        for (int i = -10; i < 21; i++) {
            for (int j = -10; j < 21; j++)
            {
                Instantiate(walls, Vector3.forward * i + Vector3.right * j, Quaternion.identity);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
