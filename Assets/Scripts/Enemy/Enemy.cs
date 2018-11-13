using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Keith.EnemyStats;
using System.Linq;

public class Enemy : MonoBehaviour {
    public EnemyStats Damage;
    public EnemyStats Speed;
    public EnemyStats Range;
    public EnemyStats Health;
    public EnemyStats Armor;

    public float healthForDebug;
    public float speedForDebug;

    public Shader SpawnShader;
    public Shader NormalShader;
    public Shader PoisonShader;
    public Shader BurnShader;
    public Shader DamageShader;


    public delegate void HealthChange();
    public event HealthChange OnHealthChange;
    public EnemyStats MaxHealth;
    public GameObject healthBarPrefab;
    public RectTransform healthPanelRect;
    private HealthBarController healthBarControls;
    private int waypointIndex;

    public List<Transform> waypoints = new List<Transform>();

    #region DoT Variables
    public float damageToTake;
    public float setTimeBetweenDamage;
    public float setDoTDuration;
    public float dotIntervalTimer;
    public float setDotIntervals;
    public float numberOfIntervals = 0;
    public float maxNumberOfIntervals;
    bool flaggedForDoT;
    public bool FlaggedForDoT
    {
        get
        {
            return flaggedForDoT;
        }

        set
        {
            flaggedForDoT = value;
        }
    }
    #endregion

    #region SLOW_VARS
    private bool isSlowed; // this relies on directed slow
    private float slowDuration = 0f; // this relies on directed slow
    private float slowSpeed = 0f;
    private float referenceSpeed = 0f;
    private float moveSpeed = 0f; // this will be a reference to the AstarAI move speed;
    #endregion

    #region STUN_VARS
    [SerializeField] private bool isStunned;
    private float stunTimer;
    private float timeToBeStunned;
    private float referenceToSpeed;
    #endregion

    bool isDead;

    public bool IsDead
    {
        get
        {
            return isDead;
        }

        set
        {
            isDead = value;
        }
    }

    // Use this for initialization
    void Awake () {
        this.GetComponent<AstarAI>().speed = Speed.Value;
        healthPanelRect = GameObject.FindGameObjectWithTag("PlayerHudCanvas").GetComponent<RectTransform>();

        generateHealthBar();
        GameObject waypointParent = GameObject.FindGameObjectWithTag("Waypoint");
        waypoints = waypointParent.GetComponentsInChildren<Transform>().ToList();
        waypoints.Remove(waypointParent.transform);

        //waypoints.Add(GameObject.FindGameObjectWithTag("Player").transform); dont want to follow the player anymore
        waypointIndex = 0;


        referenceSpeed = Speed.Value;
	}
	
	public void updateHealth () {
        this.GetComponent<AstarAI>().speed = Speed.Value;
        OnHealthChange();
        healthForDebug = Health.Value;
        speedForDebug = Speed.Value;
        GetComponent<Renderer>().material.shader = DamageShader;
        StartCoroutine(returnToDefault());
    }

    IEnumerator returnToDefault() {
        yield return new WaitForSeconds(200f);
        GetComponent<Renderer>().material.shader = NormalShader;
    }

    public Transform firstWaypoint() {
        return waypoints[waypointIndex];
    }

    public Transform nextWaypoint() {
        if (waypoints.Count-1 > waypointIndex)
        {
            waypointIndex++;
        } else
        {
            reachedEnd();
        }
        return waypoints[waypointIndex];
    }

    private void reachedEnd()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().health -= 1;
        GameObject.Find("GameManager").GetComponent<GameManager>().numberOfEnemiesActive -= 1;
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GetComponent<Renderer>().material.shader = SpawnShader;
            SetStunParms(5f);
            returnToDefault();
        }
        if (flaggedForDoT)
        {
            DoTBehavior();
        }
        if(isStunned)
        {
            StunBehaviour();
        }
        if (GetComponent<Renderer>().IsVisibleFrom(Camera.main))
            healthBarControls.Show();
        else
            healthBarControls.Hide();        
    }

    void generateHealthBar() {
        GameObject healthBar = Instantiate(healthBarPrefab) as GameObject;
        healthBarControls = healthBar.GetComponent<HealthBarController>();
        healthBarControls.SetHealthBarData(this.transform, healthPanelRect);
        healthBarControls.transform.SetParent(healthPanelRect, false);
    }



    // I think the Enemy should look after itself if is it DoTed
    #region DoT Functions
    public void SetDoTParms(float damage, float dotDuration, float dotInterval, float numberOfIntervals)
    {
        flaggedForDoT = true;
        damageToTake = damage;
        setDoTDuration = dotDuration;
        setTimeBetweenDamage = dotInterval;
        maxNumberOfIntervals = numberOfIntervals;
        GetComponent<Renderer>().material.shader = PoisonShader;
    }

    void DoTBehavior()
    {
        dotIntervalTimer += Time.deltaTime;

        if (numberOfIntervals <= maxNumberOfIntervals)
        {
            if (dotIntervalTimer > setTimeBetweenDamage)
            {
                TakeDoTDamage();
            }
        }
        else
        {
            // reset
            flaggedForDoT = false;
            dotIntervalTimer = 0f;
            setDotIntervals = 0f;
            setTimeBetweenDamage = 0f;
            maxNumberOfIntervals = 0f;
            GetComponent<Renderer>().material.shader = NormalShader;
        }
    }


    public void TakeDoTDamage()
    {
        GetComponent<Enemy>().Health.AddModifier(new StatModifier(-damageToTake, StatModType.Flat));
        updateHealth();
        dotIntervalTimer = 0f;
        numberOfIntervals++;
    }
    #endregion

    #region SLOW_FUNCTIONS
    // for directed
    public void SetSlowParms(float speed, float duration)
    {
        isSlowed = true;
    }

    // for aoe
    public void SetSlowParms(float speed)
    {
        isSlowed = true;

        slowSpeed = speed;
        Speed.AddModifier(new StatModifier(-slowSpeed, StatModType.Flat));

        GetComponent<AstarAI>().speed = Speed.Value; // tried to make it a fraction of its current speed
        
    }

    public void ResetSlowParms()
    {
        Speed.AddModifier(new StatModifier(slowSpeed, StatModType.Flat));
        isSlowed = false;
        slowSpeed = 0f;
        GetComponent<AstarAI>().speed = Speed.Value; // here we want to reset the speed of the enemy since its out of range of effect
    }
    #endregion

    #region STUN_FUNCTIONS
    public void SetStunParms(float stunTime)
    {
        timeToBeStunned = stunTime;
        isStunned = true;
        referenceToSpeed = GetComponent<AstarAI>().speed; // reference to speed, this should be affected by the other stats like slow hopefully.
        Speed.AddModifier(new StatModifier(-referenceToSpeed, StatModType.Flat));
        GetComponent<AstarAI>().speed = Speed.Value;
    }

    private void StunBehaviour()
    {
        stunTimer += Time.deltaTime;
        if(stunTimer > timeToBeStunned)
        {
            // reset
            Speed.AddModifier(new StatModifier(referenceToSpeed, StatModType.Flat));
            stunTimer = 0f;
            GetComponent<AstarAI>().speed = Speed.Value;
            isStunned = false;           
        }
    }
    #endregion
}
