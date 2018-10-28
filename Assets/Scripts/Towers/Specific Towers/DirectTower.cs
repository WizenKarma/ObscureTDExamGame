using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the regular projectile launching tower, we could add a crit chance using the procChance and powerCooldown
/// I've not yet implemented any kind of attributes with regards to 'type' or w/e we use, we can do that later using the cool stats system (thx russian)
/// </summary>

[RequireComponent(typeof(SphereCollider))]
public class DirectTower : InGameTower {

    private float elapsed = 0f;
    SphereCollider rangeSphere;
    public bool enemyIsInRange;

    public ParticleSystem directParticleSystem;

    private void Start()
    {
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

    void Attack() {
        Collider[] enemiesInRange = Physics.OverlapSphere(this.transform.position, range.Value,targetableLayers);
        foreach (Collider c in Targets(this.transform, targettype, enemiesInRange, 1)) {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {
                    ParticleSystem projectile = Instantiate(directParticleSystem, transform.position, Quaternion.identity) as ParticleSystem;
                    c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, Keith.EnemyStats.StatModType.Flat));
                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("did some damage");
                }
                else
                {
                   continue;
                }
            }
        }
    }
}
