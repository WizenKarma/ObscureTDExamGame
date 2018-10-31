using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class SplashTower : InGameTower
{
    SphereCollider rangeSphere;
    public bool enemyIsInRange;

    public GameObject bulletPref;
    public ParticleSystem muzzleFlashPrefab;

    private float roFTimer = 0f;
    private Transform barrelEnd;

    // Use this for initialization
    void Start ()
    {
        barrelEnd = gameObject.GetComponentInChildren<Transform>();
        roFTimer = fireRate.Value;
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;

    }

    // Update is called once per frame
    void Update()
    {
        roFTimer += Time.deltaTime;
        if(enemyIsInRange)
        {
            RotateToTarget();
            if(roFTimer > fireRate.Value)
            {
                Attack();
                roFTimer = 0f;
            }
        }

    }

    private void Attack()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        foreach (Collider c in Targets(this.transform, targettype, enemiesInRange, 1))
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {
                    ParticleSystem muzzleFlash = Instantiate(muzzleFlashPrefab, barrelEnd.position, Quaternion.identity) as ParticleSystem;
                    GameObject bullet = Instantiate(bulletPref, barrelEnd.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<MortarProjectile>().SetTarget(c, targettype, targetableLayers, damage.Value);

                    //c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, Keith.EnemyStats.StatModType.Flat));
                    //c.gameObject.GetComponent<Enemy>().updateHealth();
                    //print("did some damage");
                }
                else
                {
                    continue;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = true;
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
}
