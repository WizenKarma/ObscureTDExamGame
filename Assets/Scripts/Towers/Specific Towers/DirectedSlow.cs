using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DirectedSlow : InGameTower {

    private float elapsed = 0f;
    SphereCollider rangeSphere;
    public bool enemyIsInRange;
    private Transform barrelEnd;
    public GameObject bulletPref;
    public float CleaveRange = 2f;
    public float CleavePercent = 30f;
    public ParticleSystem directParticleSystem;
    public float ProjectileSpeed = 100f;

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
        foreach (Collider c in Targets(this.transform, targettype, enemiesInRange, 1))
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {
                    //ParticleSystem muzzleFlash = Instantiate(directParticleSystem, barrelEnd.position, Quaternion.identity) as ParticleSystem;
                    GameObject bullet = Instantiate(bulletPref, this.transform.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<ProjectileParticle>().setParms(c, ProjectileSpeed);
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
        Enemy enemy = c.gameObject.GetComponent<Enemy>();
        Keith.EnemyStats.StatModifier slow = new Keith.EnemyStats.StatModifier(-special.Value, Keith.EnemyStats.StatModType.PercentAdd);
        enemy.Speed.AddModifier(slow);
        DoDamage(c.gameObject.GetComponent<Enemy>(), Keith.EnemyStats.StatModType.Flat);
        StartCoroutine(slowRelease(2f, slow, c)); //this is problematic, must consider other ways to deal with it - keith
    }

    IEnumerator slowRelease(float delay, Keith.EnemyStats.StatModifier mod, Collider enemy)
    {
        yield return new WaitForSeconds(delay);
        enemy.gameObject.GetComponent<Enemy>().Speed.RemoveModifier(mod);
        enemy.gameObject.GetComponent<Enemy>().updateHealth();
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
