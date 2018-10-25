using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoTTower : InGameTower
{
    #region DOT_VARS
    /// <summary>
    /// The damage inflicted before DoT takes effect.
    /// </summary>
    [Tooltip("The damage inflicted before DoT takes effect.")]
    public bool thereIsIntialDamage;
    /// <summary>
    /// Time in seconds that each tick of Damage is applied.
    /// </summary>
    [Tooltip("Time in seconds that each tick of Damage is applied.")]
    public float timeBetweenDamage; // time between damage
    /// <summary>
    /// Is equal to Tower's Damage.Value and is the mount of damage each tick applies. Do not manually set.
    /// </summary>
    [Tooltip("Is equal to Tower's Damage.Value and is the mount of damage each tick applies. Do not manually set.")]
    public float damagePerInstance;
    /// <summary>
    /// Duration from start to last instance of damage. 
    /// </summary>
    [Tooltip("Duration from start to last instance of damage.")]
    public float damageDuration;
    /// <summary>
    /// Is equal to Damage Duration / time Between Damage. Do not manually set.
    /// </summary>
    [Tooltip("Is equal to Damage Duration / time Between Damage. Do not manually set.")]
    [SerializeField] private float numberOfIntervals;

    private float intialDamage;

    private float attackTimer;
    #endregion




    // Use this for initialization
    void Start ()
    {
        damagePerInstance = damage.Value;
        if (thereIsIntialDamage)
        {
            intialDamage = this.damage.Value;
        }
        if (timeBetweenDamage != 0f) // avoid division by zero
        {
            numberOfIntervals = damageDuration / timeBetweenDamage;
        }
    }


    // Update is called once per frame
    void Update () {
        attackTimer += Time.deltaTime;
            if (attackTimer > fireRate.Value)
            {
                AttackDoT();
            }

	}


    public void AttackDoT()
    {
        Collider[] inRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        Collider[] targets = this.Targets(this.transform, this.targettype, inRange, 1);

        foreach (Collider c in targets)
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().FlaggedForDoT != true)
                {
                    if (thereIsIntialDamage)
                    {
                        c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-intialDamage, Keith.EnemyStats.StatModType.Flat));
                        print("did intial damage");
                    }
                    c.gameObject.GetComponent<Enemy>().SetDoTParms(damagePerInstance,damageDuration, timeBetweenDamage,numberOfIntervals);

                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("Marked for DoT");
                }
            }
        }
       
        attackTimer = 0f;
    }

}
