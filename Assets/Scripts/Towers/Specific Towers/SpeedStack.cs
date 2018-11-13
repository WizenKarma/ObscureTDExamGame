using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the regular projectile launching tower, we could add a crit chance using the procChance and powerCooldown
/// I've not yet implemented any kind of attributes with regards to 'type' or w/e we use, we can do that later using the cool stats system (thx russian)
/// </summary>

[RequireComponent(typeof(SphereCollider))]
public class SpeedStack : InGameTower
{

    private float elapsed = 0f;
    SphereCollider rangeSphere;
    public bool enemyIsInRange;
    private Transform barrelEnd;
    public GameObject bulletPref;
    private GameObject target;
    private int hitCount;

    public float percentageAddPerHit;
    public ParticleSystem directParticleSystem;

    private void Start()
    {
        hitCount = 0;
        barrelEnd = gameObject.GetComponentInChildren<Transform>();
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;
    }

    public void Update()
    {
        elapsed += Time.deltaTime;
        if (enemyIsInRange)
        {
            RotateToTarget();
            if (elapsed > (fireRate.Value*(1-hitCount*percentageAddPerHit)) )
            {
                Attack();
                elapsed = 0f;
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
        }
    }

    void Attack()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        foreach (Collider c in Targets(this.transform, targettype, enemiesInRange, 1))
        {

            if (target == c.gameObject)
            {
                hitCount++;
            }
            else
            {
                hitCount = 0;
            }

            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {
                    ParticleSystem muzzleFlash = Instantiate(directParticleSystem, barrelEnd.position, Quaternion.identity) as ParticleSystem;
                    GameObject bullet = Instantiate(bulletPref, barrelEnd.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<ProjectileParticle>().setParms(c, 100f);
                    bullet.GetComponent<ProjectileParticle>().projectileDelegate = causeDamage;

                }
                else
                {
                    continue;
                }
            }
        }
    }


    void causeDamage(Collider c)
    {
        Enemy enemy = c.gameObject.GetComponent<Enemy>();
        c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, Keith.EnemyStats.StatModType.Flat));
        c.gameObject.GetComponent<Enemy>().updateHealth();
    }
}
