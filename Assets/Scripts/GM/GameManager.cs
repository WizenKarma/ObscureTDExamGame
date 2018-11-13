using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public StateMachine GameManagerStateMachine;

    public Phase[] phases;
    public PhaseBuilder.PhaseType currentPhase;

    // will improve to use this maybe
    public struct Round
    {
        public Phase BuildPhase;
        public Phase AttackPhase;
    }

    public Round[] Rounds;

    public int numberOfRounds = 0;
    public int numberOfExpectedEnemiesToSpawn = 0;
    public int numberOfEnemiesActive = 0; // this is to watch how many enemies are alive
    public int numberOfEnemiesSpawned = 0;
    public int health = 10;
    public int counter = 1;
    public int lagCounter = 0;
    public bool newPhase;

    public GameObject textBoxPrefab;
    private GameObject displayParent;
    private TextMeshProUGUI roundDisplay;
    private TextMeshProUGUI enemiesDisplay;
    private TextMeshProUGUI phaseDisplay;
    private TextMeshProUGUI healthDisplay;

    private bool isGraphRefreshed = false;
    private int round = 0;
	// Use this for initialization
	void Awake ()
    {
        phases = GetComponentsInChildren<Phase>();
        numberOfRounds = phases.Length; // rounds?
        //UI initialization
        displayParent = GameObject.Find("Round Info");
        roundDisplay = Instantiate(textBoxPrefab, displayParent.transform).GetComponent<TextMeshProUGUI>();
        enemiesDisplay = Instantiate(textBoxPrefab, displayParent.transform).GetComponent<TextMeshProUGUI>();
        phaseDisplay = Instantiate(textBoxPrefab, displayParent.transform).GetComponent<TextMeshProUGUI>();
        healthDisplay = Instantiate(textBoxPrefab, displayParent.transform).GetComponent<TextMeshProUGUI>();

        
        this.GameManagerStateMachine = GetComponent<StateMachine>();
        foreach (Phase phase in phases)
        {
            phase.enabled = false;
        }
        phases[0].enabled = true;
        GameManagerStateMachine.ChangeState(phases[0]);
        currentPhase = GameManagerStateMachine.ReturnCurrentState();
        //Debug.Log(GameManagerStateMachine.ReturnCurrentState());
        
    }

    #region ATTACK_PHASE_BEHAVIOUR
    private void CheckEnemies()
    {
        if (currentPhase == PhaseBuilder.PhaseType.Attack)
        {
            if (numberOfEnemiesSpawned == numberOfExpectedEnemiesToSpawn) // then all enemies for round has spawned
            {
                if (numberOfEnemiesActive <= 0) // then all enemies for round has died
                {
                    EndOfAttackPhase(); // change to next build phase;
                    ResetEnemyCounters(); // safety first
                }
            }
        }
    }

    private void ResetEnemyCounters()
    {
        numberOfExpectedEnemiesToSpawn = 0;
        numberOfEnemiesActive = 0; 
        numberOfEnemiesSpawned = 0;
    }
    #endregion

    private void Start()
    {
       
    }

    // Update is called once per frame
    void Update ()
    {
        if (currentPhase == PhaseBuilder.PhaseType.Attack)
        {
            CheckEnemies();
        }
        if (currentPhase == PhaseBuilder.PhaseType.Combine && isGraphRefreshed == false)
        {
            //recalculate the graph
            AstarPath.active.Scan();
            isGraphRefreshed = true;
        }
        //if (Input.GetKeyDown(KeyCode.Return)) // removed newPhase for testing purpose rn
        //{
        //    ChangeBehaviour();
            
        //}

        roundDisplay.text = "Round : " + round + "/" + numberOfRounds;
        enemiesDisplay.text = "Enemies : " + numberOfEnemiesActive;
        phaseDisplay.text = "Phase : " + currentPhase.ToString();
        healthDisplay.text = "Lives : " + health;
    }

    public void ChangeBehaviour()
    {
        isGraphRefreshed = false;
        //Debug.Log(GameManagerStateMachine.ReturnCurrentState());
        if (counter == phases.Length)
        {
            phases[lagCounter].enabled = false;
            return;
        }
        phases[lagCounter].enabled = false;
        lagCounter = counter;
        phases[counter].enabled = true;
        GameManagerStateMachine.ChangeState(phases[counter]);
        counter++;
        currentPhase = GameManagerStateMachine.ReturnCurrentState();
        if (currentPhase == PhaseBuilder.PhaseType.Build)
            round += 1;
    }

    public void EndOfAttackPhase()
    {
        // this is called from the attack phase end
        // this occurs when all enemies are dead
        ChangeBehaviour();
    }

    //private void Singleton()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
