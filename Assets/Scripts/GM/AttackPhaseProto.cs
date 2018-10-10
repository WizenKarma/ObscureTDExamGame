using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPhaseProto : Phase
{
    public PhaseBuilder thisPhase;

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

        if ((waveCounter > 0))
        {
            if (enemyCounter > 0)
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
        thePlayer = FindObjectOfType<PlayerControllerScript>();
        this.gameManager = FindObjectOfType<GameManager>();
        phaseName = thisPhase.PhaseName;
        numberOfEnemies = thisPhase.NumberOfEnemies;
        numberOfWaves = thisPhase.NumberOfWaves;
        timeBetweenEnemySpawn = thisPhase.TimeBetweenEnemySpawn;
        timeBetweenWaves = thisPhase.TimeBetweenWaves;
        phaseType = thisPhase.phaseType;
        this.enemyToSpawn = this.thisPhase.EnemyToSpawn;
        enemyCounter = numberOfEnemies;
        waveCounter = numberOfWaves;
        waveTimer = timeBetweenWaves;
        if (!thePlayer)
        {
            Debug.Log("There is no player!");
        }
    }
}
