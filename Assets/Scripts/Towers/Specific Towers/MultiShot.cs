using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MultiShot : InGameTower {

    private float elapsed = 0f;
    SphereCollider rangeSphere;
    public bool enemyIsInRange;
    private Transform barrelEnd;
    public GameObject bulletPref;
    public ParticleSystem directParticleSystem;

    // Use this for initialization
    void Start()
    {
        barrelEnd = gameObject.GetComponentInChildren<Transform>();
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;
    }

    void Attack()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        print(Targets(this.transform, targettype, enemiesInRange, 3).Length);
        foreach (Collider c in Targets(this.transform, targettype, enemiesInRange, 3))
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {
                    //ParticleSystem muzzleFlash = Instantiate(directParticleSystem, barrelEnd.position, Quaternion.identity) as ParticleSystem;
                    GameObject bullet = Instantiate(bulletPref, barrelEnd.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<ProjectileParticle>().setParms(c, projectileSpeed);
                    bullet.GetComponent<ProjectileParticle>().projectileDelegate = causeDamage;
                    //bullet.GetComponent<ProjectileParticle>().projectileDelegate(c);
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
        DoDamage(c.gameObject.GetComponent<Enemy>(), Keith.EnemyStats.StatModType.Flat);
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

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (enemyIsInRange)
        {
            RotateToTarget();
            if (elapsed > fireRate.Value)
            {
                Attack();
                elapsed = 0f;
            }
        }
    }
}
