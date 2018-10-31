using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I know rewritten things are a waste of time but i wanted to get quality done for beta - Ced
// This tower will be a combine of the DoT and AoE Slow to produce the slow and dot Fragmentation
[RequireComponent(typeof(SphereCollider))]
public class SlowDotTower : InGameTower
{
    #region DOT_VARS

    [Tooltip("Time in seconds that each tick of Damage is applied.")]
    public float timeBetweenDamage; // time between damage

    [Tooltip("Is equal to Tower's Damage.Value and is the mount of damage each tick applies. Do not manually set.")]
    public float damagePerInstance;

    [Tooltip("Duration from start to last instance of damage.")]
    public float damageDuration;

    [Tooltip("Is equal to Damage Duration / time Between Damage. Do not manually set.")]
    [SerializeField] private float numberOfIntervals;

    private float intialDamage;
    #endregion

    SphereCollider rangeSphere;
    public bool enemyIsInRange;
    public GameObject muzzleFlashPrefab;

    [SerializeField] private float slowSpeed = 0f;
    private Transform barrelEnd;

    private float roFTimer = 0f;


    // Use this for initialization
    void Start ()
    {
        barrelEnd = gameObject.GetComponentInChildren<Transform>();
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;
        intialDamage = this.damage.Value;
        if (timeBetweenDamage != 0f) // avoid division by zero
        {
            numberOfIntervals = damageDuration / timeBetweenDamage;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        roFTimer += Time.deltaTime;
        if(enemyIsInRange)
        {
            if(roFTimer > fireRate.Value)
            {
                Attack();
                roFTimer = 0f;
            }

        }
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = true;
            
            other.gameObject.GetComponent<Enemy>().SetSlowParms(slowSpeed);
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = true;
            //other.gameObject.GetComponent<Enemy>().SetSlowParms(slowSpeed); This should not be here
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = false;
            other.gameObject.GetComponent<Enemy>().ResetSlowParms();
        }
    }

    private void Attack()
    {
        Collider[] inRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        Collider[] targets = this.Targets(this.transform, this.targettype, inRange, 1);

        foreach (Collider c in targets)
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().FlaggedForDoT != true)
                {
                    c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-intialDamage, Keith.EnemyStats.StatModType.Flat));
                    print("did intial damage");
                    

                    GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, barrelEnd.position, Quaternion.identity) as GameObject;
                    //GameObject bullet = Instantiate(bulletPref, barrelEnd.position, Quaternion.identity) as GameObject;
                    //bullet.GetComponent<ProjectileParticle>().setParms(c, 100f);

                    c.gameObject.GetComponent<Enemy>().SetDoTParms(damagePerInstance, damageDuration, timeBetweenDamage, numberOfIntervals);

                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("Marked for DoT");
                }
            }
        }
    }
}
