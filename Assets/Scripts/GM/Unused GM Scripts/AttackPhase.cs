using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPhase : MonoBehaviour, IState
{
    private GameObject enemyToSpawn;
    private int wavesOfEnemies;
    private int numberOfEnemiesToSpawn;
    private float spawnDelayOfEnemies;
    //private GameObject[] spawnPointsForEnemies;
    private Transform spawnPointForEnemies;
    private GameManager gm;
    private float timer;

    // want to define each attacking phase and set up each parameter

    //constructor for each phase
    public AttackPhase(int enemyWaves, int enemyCount, float spawnDelay, GameObject enemySpawn /*, GameObject[] spawnPoints*/, Transform spawnPoint, GameManager gm)
    {
        this.wavesOfEnemies = enemyWaves;
        this.numberOfEnemiesToSpawn = enemyCount;
        this.spawnDelayOfEnemies = spawnDelay;
        // this.spawnPointsForEnemies = spawnPoints;
        this.enemyToSpawn = enemySpawn;
        this.spawnPointForEnemies = spawnPoint;
        this.gm = gm;
    }

    public void Execute()
    {
        // in here we want to spawn an enemy at each spawn point at x seconds
        // ah
        //InvokeRepeating("SpawnEnemy", spawnDelayOfEnemies, numberOfEnemiesToSpawn);
        Debug.Log("executed");
    }

    public void Enter()
    {
        // will essentially be like a start function
        Debug.Log("entered");
        

    }

    public void Exit()
    {
        Debug.Log("exited");
    }

    private IEnumerator SpawnDelayFunction(float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);
        //spawn an enemy
        SpawnEnemy();
        //GameManager.Instance.numberOfEnemiesActive++;
    }

    private void SpawnEnemy()
    {
        if (this.numberOfEnemiesToSpawn > 0)
        {
            //GameObject.Instantiate(this.enemyToSpawn, spawnPointForEnemies.position, Quaternion.identity);
            gm.numberOfEnemiesActive++;
            //gm.GameManagerStateMachine.ExecuteStateUpdate();
            this.numberOfEnemiesToSpawn--;
            Debug.Log(this.numberOfEnemiesToSpawn);
            this.timer = 0f;
        }
        else
        {
            return;
        }
    }



    // Update is called once per frame
    //void Update()
    //{
    //    this.timer = this.timer + Time.deltaTime;
    //    if (this.timer > spawnDelayOfEnemies)
    //    {
    //        SpawnEnemy();
    //    }
    //}
}
