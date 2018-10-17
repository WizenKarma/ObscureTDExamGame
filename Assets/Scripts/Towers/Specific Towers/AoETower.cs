using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoETower : InGameTower
{
    public enum EffectType
    {
        Damage = 200,
        // other AoE effects?
    };

    public EffectType thisEffect;
    private float timerVar;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        timerVar = Time.deltaTime;
		if (timerVar > fireRate.Value)
        {
            ApplyAoE();
        }
	}

    // Fn checks if the array from Targets is going to deal damage to an Enemy
    // Error checking i think?
    Transform TransformOfTarget (Collider[] targetsToCheck)
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

    // could this be under InGameTowers as an AuxFn for say, a DOT AOE tower or a SLOW AOE tower?
    public void ApplyAoE()
    {
        Collider[] inRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        Collider[] targets = this.Targets(this.transform, this.targettype, inRange, 1);

        // error checking here
        Transform targetTransform = TransformOfTarget(targets);
        

        // might need a separate AoE range around the target?

        Collider[] targetsToAoE = Physics.OverlapSphere(targetTransform.position, range.Value, targetableLayers);

        if (thisEffect == EffectType.Damage)
        {
            foreach (Collider c in targetsToAoE)
            {
                if (c.gameObject.GetComponent<Enemy>() as Enemy)
                {
                    c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, Keith.EnemyStats.StatModType.Flat));
                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("did AoE damage");
                }
            }
        }
        timerVar = 0f;
    }
}
