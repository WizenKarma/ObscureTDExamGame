using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour
{
    protected GameManager gameManager;

    //protected PlayerControllerScript thePlayer; //removed for tut
    protected CameraController thePlayer;
    // protected PlayerControllerTutorial4 thePlayer;

    public PhaseBuilder phase;

    protected string phaseName = "";


    protected int numberOfEnemies;
    protected int enemyCounter;

    protected int numberOfWaves;
    protected int waveCounter;

    protected float timeBetweenEnemySpawn;

    protected float timeBetweenWaves; // time between the last spawn of wave x and first spawn of wave x+1

    public PhaseBuilder.PhaseType phaseType;

    protected GameObject enemyToSpawn;
    public GameObject enemySpawnPoint;

    // dunno what to do with these
    /*protected AttackPhase attackPhase;
    protected BuildPhase buildPhase;*/
    protected float enemyTimer;
    protected float waveTimer;
    protected int counter = 0;
    protected bool newWave = true;

    #region Attack Phase
    
    /*  If there is 2 enemy types
     *  we will want to spawn x enemy y times
     *  and z enemy w times.
     * 
     *  for that matter how many different types
     *  can we have?
     *  
     *  how many spawn points will be active?
     */


    void SpawnEnemy()
    {
        
        GameObject enemyClone = Instantiate(enemyToSpawn, enemySpawnPoint.transform.position, Quaternion.identity);
        enemyTimer = 0f;
        enemyCounter--;
        gameManager.numberOfEnemiesActive++;
        gameManager.numberOfEnemiesSpawned++;
        // then when an enemy dies it tells the gm to decrease
        // if all the active enemies are dead the wave ends and changes phase
        
    }

    void Test()
    {
        if (gameManager.numberOfEnemiesActive == numberOfEnemies * numberOfWaves)
        {
            ForceChange();
        }
    }



    void CheckWave()
    {
        enemyTimer += Time.deltaTime;

        if ( (waveCounter > 0) )
        {
            if (enemyCounter > 0 )
            {
                if (enemyTimer > timeBetweenEnemySpawn)
                {
                    Debug.Log("This is wave:" + waveCounter);
                    SpawnEnemy();
                    waveTimer = 0;
                }
            }
            else
            {
                Debug.Log("waiting");
                
                newWave = false;
                
            }
        }
        StartWaveTimer();
    }

    void StartWaveTimer()
    {
        if (!newWave)
        {
            waveTimer += Time.deltaTime;
            if (waveTimer > timeBetweenWaves)
            {
                waveCounter--;
                enemyCounter = numberOfEnemies;
                newWave = true;
            }
        }
        else
        {
            waveTimer = 0;
            //Test();////////////
            return;
        }
        
    }
    #endregion

    protected void SetParamaters()
    {
        //thePlayer = FindObjectOfType<PlayerControllerTutorial4>();
        //thePlayer = FindObjectOfType<PlayerControllerScript>();
        thePlayer = FindObjectOfType<CameraController>();
        this.gameManager = FindObjectOfType<GameManager>();
        phaseName = phase.PhaseName;
        numberOfEnemies = phase.NumberOfEnemies;
        numberOfWaves = phase.NumberOfWaves;
        timeBetweenEnemySpawn = phase.TimeBetweenEnemySpawn;
        timeBetweenWaves = phase.TimeBetweenWaves;
        phaseType = phase.phaseType;
        this.enemyToSpawn = this.phase.EnemyToSpawn;
        enemyCounter = numberOfEnemies;
        waveCounter = numberOfWaves;
        waveTimer = timeBetweenWaves;
        if (!thePlayer)
        {
            Debug.Log("There is no player!");
        }
    }

    #region Build Phase
    private void BuildStuff()
    {
        gameManager.newPhase = false;
        //if(thePlayer != null)
        //thePlayer.SetBuild(true);
        // here we want to perform various functions
        // details include the house keeping to maintain
        // that the player can do, ie after 5 towers are place,
        // keep a tower then procced to the next state, of combining
        if(thePlayer.towerCounter == 5)
        {
            gameManager.currentPhase = PhaseBuilder.PhaseType.Combine;
            thePlayer.ResetTowerCount();

        }
    }
    #endregion

    public void Enter()
    {
        SetParamaters();
        Debug.Log("Entering current state:" + this.phaseName);

        if (phaseType == PhaseBuilder.PhaseType.Build)
        {
            thePlayer.TowerReset();
            BuildStuff();
            //gameManager.ChangeBehaviour();
        }
        if (phaseType == PhaseBuilder.PhaseType.Attack)
        {
            gameManager.numberOfExpectedEnemiesToSpawn = numberOfEnemies * numberOfWaves;
        }
    }


    public void Execute()
    {
        // not necessary?

    }

    protected void ForceChange()
    {
        if (phaseType == PhaseBuilder.PhaseType.Attack)
        {
            gameManager.ChangeBehaviour();
        }
    }

    public void Exit()
    {
        
        Debug.Log("Leaving current state:" + this.phaseName);
        
    }

    void Update()
    {
        if (phaseType == PhaseBuilder.PhaseType.Attack)
        {
            CheckWave();
        }
        if (phaseType == PhaseBuilder.PhaseType.Build)
        {
            BuildStuff();
        }
    }
}
