using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hologramShader : MonoBehaviour {
    public static hologramShader instance;
    public Material hologramMat;
    public Shader hologram;
    public Material towerMat;
    private Phase CurrPhase;
    Renderer rend;
	// Use this for initialization
	void Start () {
      
	}
	
	// Update is called once per frame
	void Update () {
       //if (GameObject.FindGameObjectWithTag Phase. == 'Build'){ }
		
	}

     public void hologramApp()
    {
        rend = this.gameObject.GetComponent<Renderer>();
        rend.material = new Material(hologram);
    }
}
