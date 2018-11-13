using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Keith.Towers;

public class InGameTower : MonoBehaviour {

    public Tower tower;
    public TowerStats damage;
    public TowerStats range;
    public TowerStats fireRate;
    public TowerStats cooldown;
    public TowerStats procChance;
    public TowerStats special;

    public float projectileSpeed;
    public LayerMask targetableLayers;
    public Tower.TargetType targettype;
    List<Combiner> possibleRecipes = new List<Combiner>();
    
    private void Awake()
    {
        tower.TargetTower = this.gameObject;
        projectileSpeed = tower.projectileSpeed;
        this.name = tower.name;
        damage = tower.Damage;
        range = tower.Range;
        fireRate = tower.FireRate;
        cooldown = tower.PowerCooldown;
        procChance = tower.ProcChance;
        special = tower.Special;
        targetableLayers = tower.targetableLayers;
        targettype = tower.targettype;
        if (tower.AestheticMesh)
            this.GetComponent<MeshFilter>().mesh = tower.AestheticMesh.GetComponent<MeshFilter>().sharedMesh;
    }

    public void addRecipe(Combiner combiner) {
        if (!possibleRecipes.Contains(combiner))
            possibleRecipes.Add(combiner);
    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }

    public void DoDamage(Enemy enemy, Keith.EnemyStats.StatModType type)
    {
        enemy.Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, type, enemy.Armor.Value));
        enemy.updateHealth();
    }
    
    /*void OnDrawGizmosSelected()
 {
     Gizmos.color = Color.black;
     Gizmos.DrawWireSphere(transform.position, Range.BaseValue);
 }*/

    /*----------------------- UTILITY FUNCTIONS FOR ALL TOWERS -----------------------*/
    #region UTILITY_FUNCTIONS_ALL_TOWERS

    public Collider[] Targets(Transform towerTransform, Tower.TargetType type, Collider[] inRange, int count = default(int))
    {
        switch (type)
        {
            case Tower.TargetType.Closest:
                {
                    inRange = inRange.OrderBy(x => Vector3.Distance(towerTransform.position, x.transform.position)).ToArray();
                    return inRange.Take(count).ToArray();
                }
            case Tower.TargetType.Lowest:
                {
                    inRange = inRange.OrderBy(x => x.GetComponent<Enemy>().Health).ToArray();
                    return inRange.Take(count).ToArray();
                }
            case Tower.TargetType.Furthest:
                {
                    inRange = inRange.OrderBy(x => Vector3.Distance(towerTransform.position, x.transform.position)).ToArray();
                    inRange.Reverse();
                    return inRange.Take(count).ToArray();
                }
            case Tower.TargetType.Around:
                {
                    inRange = inRange.OrderBy(x => Vector3.Distance(towerTransform.position, x.transform.position)).ToArray();
                    return inRange;
                }
        }
        return null;
    }

    // Fn checks if the array from Targets is going to deal damage to an Enemy
    // Error checking i think?
    protected Transform TransformOfTarget(Collider[] targetsToCheck)
    {
        foreach (Collider c in targetsToCheck)
        {
            if (c.gameObject.GetComponent<Enemy>())
            {
                return c.gameObject.GetComponent<Enemy>().transform;
            }
        }
        return null;
    }

    protected void RotateToTarget()
    {
        Collider[] inRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        Transform target = TransformOfTarget(inRange);
        if (target != null)
        {
            //Transform target = inRange[0].GetComponent<Transform>(); // first enemy?
            Vector3 vecToTarget = target.position - this.transform.position;
            transform.LookAt(target);
        }
    }

    //protected void RotateToTarget()
    //{
    //    Vector3 targetToLookAt = 
    //}
    #endregion
}
