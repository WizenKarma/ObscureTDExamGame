using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour {
    public static event Action<Health> OnHealthAdded = delegate { };
    public static event Action<Health> OnHealthRemoved = delegate { };

    public int maxHealth = 100;

    public int CurrentHealth { get; private set; }

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable()
    {
        CurrentHealth = maxHealth;
        OnHealthAdded(this);
    }

    public void ModifyHealth() {
        float CurrentHealthPct = (float)CurrentHealth / (float)maxHealth;
        OnHealthPctChanged(CurrentHealthPct);
    }

    private void OnDisable()
    {
        OnHealthRemoved(this);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
