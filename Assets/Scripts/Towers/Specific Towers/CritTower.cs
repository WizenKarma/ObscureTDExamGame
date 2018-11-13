using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CritTower : InGameTower
{

    private float elapsed = 0f;
    SphereCollider rangeSphere;
    public bool enemyIsInRange;
    private Transform barrelEnd;
    public GameObject bulletPref;
    public float CleaveRange = 2f;
    public float CleavePercent = 30f;
    public ParticleSystem directParticleSystem;
    public float critChance = 0.2f;
    public float critMultiplier = 3f;
    // Use this for initialization
    void Start()
    {
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
                    GameObject bullet = Instantiate(bulletPref, this.transform.position, Quaternion.identity) as GameObject;
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
        float random = Random.value;
        float multiplier = 1f;
        if (random < 1f - critChance)
        {
            multiplier = critMultiplier;
        }

        c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value * multiplier, Keith.EnemyStats.StatModType.Flat, c.gameObject.GetComponent<Enemy>().Armor.Value));
        c.gameObject.GetComponent<Enemy>().updateHealth();
        //DoDamage(c.gameObject.GetComponent<Enemy>(), Keith.EnemyStats.StatModType.Flat);
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
