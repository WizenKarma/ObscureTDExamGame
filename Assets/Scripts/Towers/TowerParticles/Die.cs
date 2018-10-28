using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour {

    private ParticleSystem thisParticleSystem;
    private float timer;

	// Use this for initialization
	void Start ()
    {
        thisParticleSystem = GetComponent<ParticleSystem>();
        thisParticleSystem.Play();

    }

    void die()
    {
        thisParticleSystem.Stop();
        Destroy(this.gameObject);
    }
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if(timer > 3f)
        {
            die();
        }
		
	}
}
