using System.Collections;
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
    public float setDoTInterval;
    public float setDoTDuration;
    public float dotIntervalTimer;
    public float setDotIntervals;
    public float numberOfIntervals = 1;
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
        healthPanelRect = GameObject.FindGameObjectWithTag("PlayerHudCanvas").GetComponent<RectTransform>();

        generateHealthBar();
        GameObject waypointParent = GameObject.FindGameObjectWithTag("Waypoint");
        waypoints = waypointParent.GetComponentsInChildren<Transform>().ToList();
        waypoints.Remove(waypointParent.transform);

        //waypoints.Add(GameObject.FindGameObjectWithTag("Player").transform); dont want to follow the player anymore
        waypointIndex = 0;
	}
	
	// Update is called once per frame
	public void updateHealth () {
            OnHealthChange();
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
        if (flaggedForDoT)
        {
            DoTBehavior();
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
        setDoTInterval = dotInterval;
        maxNumberOfIntervals = numberOfIntervals;
    }

    void DoTBehavior()
    {
        dotIntervalTimer += Time.deltaTime;

        if (numberOfIntervals < maxNumberOfIntervals)
        {
            if (dotIntervalTimer > setDoTInterval)
            {
                TakeDoTDamage();
            }
        }
        else
        {
            // reset
            flaggedForDoT = false;
            dotIntervalTimer = 0f;
            setDotIntervals = 1f;
            
            return;
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
}
