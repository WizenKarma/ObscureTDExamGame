using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DoTTower : InGameTower
{
    #region DOT_VARS
    /// <summary>
    /// The damage inflicted before DoT takes effect.
    /// </summary>
    [Tooltip("The damage inflicted before DoT takes effect.")]
    public bool thereIsIntialDamage;
    /// <summary>
    /// Time in seconds that each tick of Damage is applied.
    /// </summary>
    [Tooltip("Time in seconds that each tick of Damage is applied.")]
    public float timeBetweenDamage; // time between damage
    /// <summary>
    /// Is equal to Tower's Damage.Value and is the mount of damage each tick applies. Do not manually set.
    /// </summary>
    [Tooltip("Is equal to Tower's Damage.Value and is the mount of damage each tick applies. Do not manually set.")]
    public float damagePerInstance;
    /// <summary>
    /// Duration from start to last instance of damage. 
    /// </summary>
    [Tooltip("Duration from start to last instance of damage.")]
    public float damageDuration;
    /// <summary>
    /// Is equal to Damage Duration / time Between Damage. Do not manually set.
    /// </summary>
    [Tooltip("Is equal to Damage Duration / time Between Damage. Do not manually set.")]
    [SerializeField] private float numberOfIntervals;

    private float intialDamage;

    private float attackTimer;
    #endregion

    SphereCollider rangeSphere;
    public bool enemyIsInRange;

    public ParticleSystem directParticleSystem;

    private Transform barrelEnd;
    public GameObject bulletPref;
    


    // Use this for initialization
    void Start ()
    {
        barrelEnd = gameObject.GetComponentInChildren<Transform>();
        attackTimer = fireRate.Value;
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;

        damagePerInstance = damage.Value;
        if (thereIsIntialDamage)
        {
            intialDamage = this.damage.Value;
        }
        if (timeBetweenDamage != 0f) // avoid division by zero
        {
            numberOfIntervals = damageDuration / timeBetweenDamage;
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
        if(other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = false;
        }
    }



    // Update is called once per frame
    void Update ()
    {
        attackTimer += Time.deltaTime;
        if (enemyIsInRange)
        {
            if (attackTimer > fireRate.Value)
            {
                RotateToTarget();
                AttackDoT();
            }
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
                if (c.gameObject.GetComponent<Enemy>().FlaggedForDoT != true)
                {
                    if (thereIsIntialDamage)
                    {
                        c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-intialDamage, Keith.EnemyStats.StatModType.Flat));
                        print("did intial damage");
                    }

                    ParticleSystem muzzleFlash = Instantiate(directParticleSystem, barrelEnd.position, Quaternion.identity) as ParticleSystem;
                    GameObject bullet = Instantiate(bulletPref, barrelEnd.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<ProjectileParticle>().setParms(c, 100f);

                    c.gameObject.GetComponent<Enemy>().SetDoTParms(damagePerInstance,damageDuration, timeBetweenDamage,numberOfIntervals);

                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("Marked for DoT");
                }
                else
                {
                    continue;
                }
            }
        }
       
        attackTimer = 0f;
    }

}
