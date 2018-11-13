using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TowerBuff : InGameTower
{
    public enum EffectType
    {
        Damage = 200,
        Buff = 300,
        // other AoE effects?
    };

    public EffectType thisEffect;
    private float timerVar;
    private bool isSet;

    public bool buffDamage;
    public bool buffSpeed;
    public bool buffRange;

    public float damageBuff;
    public float speedBuff;
    public float rangeBuff;

    SphereCollider rangeSphere;

    List<GameObject> buffedTowers = new List<GameObject>();
    //public bool enemyIsInRange;
    //private Transform barrelEnd;

    //public ParticleSystem directParticleSystem;
    //public GameObject bulletPref;

    // Use this for initialization
    void Start()
    {
        isSet = false;
        //barrelEnd = transform.Find("BarrelEnd");
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;
    }

    // Update is called once per frame
    void Update()
    {
        /*RotateToTarget();
          timerVar += Time.deltaTime;
          if (timerVar > fireRate.Value)
          {
              ApplyAoE();
              timerVar = 0f;
          }*/
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gm.currentPhase == PhaseBuilder.PhaseType.Attack && !isSet)
        {
            ApplyAoE();
            isSet = true;
        } else if (gm.currentPhase != PhaseBuilder.PhaseType.Attack)
        {
            isSet = false;
        }
    }

    // could this be under InGameTowers as an AuxFn for say, a DOT AOE tower or a SLOW AOE tower?
    public void ApplyAoE()
    {
        Collider[] inRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        Collider[] targets = this.Targets(this.transform, this.targettype, inRange, 1);

        // error checking here
        Transform targetTransform = TransformOfTarget(targets);


        // might need a separate AoE range around the target?

        //Collider[] targetsToAoE = Physics.OverlapSphere(targetTransform.position, range.Value, targetableLayers);

        if (thisEffect == EffectType.Damage)
        {
            foreach (Collider c in targets)
            {
                //if (c.gameObject.GetComponent<InGameTower>() as InGameTower && !buffedTowers.Contains(c.gameObject))
                //{
                    //c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, Keith.EnemyStats.StatModType.Flat));
                    if (buffDamage)
                        c.gameObject.GetComponent<InGameTower>().damage.AddModifier(new Keith.Towers.TowerModifier(damageBuff, Keith.Towers.StatModType.PercentAdd));
                    if (buffSpeed)
                        c.gameObject.GetComponent<InGameTower>().damage.AddModifier(new Keith.Towers.TowerModifier(speedBuff, Keith.Towers.StatModType.PercentAdd));
                    if (buffRange)
                        c.gameObject.GetComponent<InGameTower>().damage.AddModifier(new Keith.Towers.TowerModifier(rangeBuff, Keith.Towers.StatModType.PercentAdd));
                    //c.gameObject.GetComponent<Enemy>().updateHealth();
                    buffedTowers.Add(c.gameObject);
                    print("Applied Stat Modifier");
                //}
            }
        }
    }
}
