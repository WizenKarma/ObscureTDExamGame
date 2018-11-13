using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the regular projectile launching tower, we could add a crit chance using the procChance and powerCooldown
/// I've not yet implemented any kind of attributes with regards to 'type' or w/e we use, we can do that later using the cool stats system (thx russian)
/// </summary>

[RequireComponent(typeof(SphereCollider))]
public class ArmourBurnSlow : InGameTower
{

    private float elapsed = 0f;
    SphereCollider rangeSphere;
    public bool enemyIsInRange;
    private Transform barrelEnd;
    public GameObject bulletPref;

  
    public float armorBurnPercentage;
    public ParticleSystem directParticleSystem;

    private void Start()
    {
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
            if (elapsed > fireRate.Value)
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
        foreach (Collider c in Targets(this.transform, targettype, enemiesInRange, 100))
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {
                    ParticleSystem muzzleFlash = Instantiate(directParticleSystem, barrelEnd.position, Quaternion.identity) as ParticleSystem;
                    GameObject bullet = Instantiate(bulletPref, barrelEnd.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<ProjectileParticle>().setParms(c, projectileSpeed);
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
        enemy.Armor.AddModifier(new Keith.EnemyStats.StatModifier(-armorBurnPercentage, Keith.EnemyStats.StatModType.PercentMult));
        enemy.Armor.AddModifier(new Keith.EnemyStats.StatModifier(-armorBurnPercentage, Keith.EnemyStats.StatModType.PercentMult));
        DoDamage(c.gameObject.GetComponent<Enemy>(), Keith.EnemyStats.StatModType.Flat);
    }
}
