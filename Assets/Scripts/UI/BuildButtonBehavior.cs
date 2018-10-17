using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButtonBehavior : MonoBehaviour {
    public Tower towerToBuild;
	// Use this for initialization
	void Start () {
		
	}	
	// Update is called once per frame
	void Update () {
		
	}

    public void buildTower() {
        GameObject player = GameObject.Find("Player_Proto");
        //player.GetComponent<PlayerControllerScript>().setBuildTower(towerToBuild);
    }
}
