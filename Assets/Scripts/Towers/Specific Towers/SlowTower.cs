using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I opted to make a new script entirely, I didn't want to clutter the AoE tower script - Ced

[RequireComponent(typeof(SphereCollider))]
public class SlowTower : InGameTower
{
    public enum SlowType
    {
        AoE = 100,
        Directed = 300,
    }

    SphereCollider rangeSphere;
    public bool enemyIsInRange;

    [Tooltip("Type of Slow effect. Set to AoE by default")]
    [SerializeField] private SlowType thisSlowType = SlowType.AoE; // Aoe by default

    private float slowValue; // this uses the tower damage var


    // Use this for initialization
    void Start ()
    {
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;
        slowValue = damage.Value;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(thisSlowType == SlowType.Directed)
        {
           // directed slow tower stuff here;
        }
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = true;
            if(thisSlowType == SlowType.AoE)
            {
                other.gameObject.GetComponent<Enemy>().SetSlowParms(slowValue);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = false;
            if (thisSlowType == SlowType.AoE)
            {
                other.gameObject.GetComponent<Enemy>().ResetSlowParms();
            }
        }
    }
}
