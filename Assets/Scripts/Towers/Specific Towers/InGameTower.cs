using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Keith.Towers;

public class InGameTower : MonoBehaviour {

    public Tower tower;
    protected TowerStats damage;
    protected TowerStats range;
    protected TowerStats fireRate;
    protected TowerStats cooldown;
    protected TowerStats procChance;
    protected LayerMask targetableLayers;
    protected Tower.TargetType targettype;
    List<Combiner> possibleRecipes = new List<Combiner>();
    
    private void Awake()
    {
        tower.TargetTower = this.gameObject;
        this.name = tower.name;
        damage = tower.Damage;
        range = tower.Range;
        fireRate = tower.FireRate;
        cooldown = tower.PowerCooldown;
        procChance = tower.ProcChance;
        targetableLayers = tower.targetableLayers;
        targettype = tower.targettype;
        if (tower.AestheticMesh)
            this.GetComponent<MeshFilter>().mesh = tower.AestheticMesh.GetComponent<MeshFilter>().sharedMesh;
    }

    public void addRecipe(Combiner combiner) {
        if (!possibleRecipes.Contains(combiner))
            possibleRecipes.Add(combiner);
    }

    /*void OnDrawGizmosSelected()
 {
     Gizmos.color = Color.black;
     Gizmos.DrawWireSphere(transform.position, Range.BaseValue);
 }*/

    /*----------------------- UTILITY FUNCTIONS FOR ALL TOWERS -----------------------*/

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

}
