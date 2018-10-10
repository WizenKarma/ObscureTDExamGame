using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the regular projectile launching tower, we could add a crit chance using the procChance and powerCooldown
/// I've not yet implemented any kind of attributes with regards to 'type' or w/e we use, we can do that later using the cool stats system (thx russian)
/// </summary>
public class DirectTower : InGameTower {

    private float elapsed = 0f;

    public void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > fireRate.Value) {
            Attack();
            elapsed = 0f;
        }
    }

    void Attack() {
        Collider[] enemiesInRange = Physics.OverlapSphere(this.transform.position, range.Value,targetableLayers);
        foreach (Collider c in Targets(this.transform, targettype, enemiesInRange, 1)) {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {
                    c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, Keith.EnemyStats.StatModType.Flat));
                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("did some damage");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
