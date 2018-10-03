using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTTower : InGameTower
{

    public bool thereIsIntialDamage;
    public float intervalBetweenInstances; // time between damage
    public float damagePerInstance; 
    public float damageDuration;
    public float numberOfIntervals;

    private float intialDamage;

    private float attackTimer;


    // Use this for initialization
    void Start ()
    {
        if (thereIsIntialDamage)
        {
            intialDamage = this.damage.Value;
        }
        if (numberOfIntervals == 0f) // whats the point of this Ced? - Ced
        {
            numberOfIntervals = damageDuration / intervalBetweenInstances;
        }
	}


	
	// Update is called once per frame
	void Update () {
        attackTimer += Time.deltaTime;
        if (attackTimer > fireRate.Value)
        {
            AttackDoT();
        }
	}


    public void AttackDoT()
    {
        Collider[] inRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        Collider[] targets = this.Targets(this.transform, this.targettype, inRange, 1);

        foreach (Collider c in targets)
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (thereIsIntialDamage)
                {
                    c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-intialDamage, Keith.EnemyStats.StatModType.Flat));
                    print("did intial damage");
                }
                c.gameObject.GetComponent<Enemy>().SetDoTParms(damage.Value, damageDuration, intervalBetweenInstances);
                
                c.gameObject.GetComponent<Enemy>().updateHealth();
                print("Marked for DoT");
            }
        }
       
        attackTimer = 0f;
    }

}
