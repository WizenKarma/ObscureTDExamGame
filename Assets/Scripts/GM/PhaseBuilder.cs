using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PhaseBuilder : ScriptableObject
{
    [SerializeField]
    public string PhaseName = "";
    public int NumberOfEnemies;
    public int NumberOfWaves;
    public float TimeBetweenEnemySpawn;
    public float TimeBetweenWaves;
    public PhaseType phaseType;
    public GameObject EnemyToSpawn;
    /*public AttackPhase AttackPhase;
    public BuildPhase BuildPhase;*/

    public enum PhaseType
    {
        Attack = 100,
        Build = 200,
        Combine = 300,
        Tutorial1 = 400,
        Tutorial2 = 401,
        Tutorial3 = 402,
        Tutorial4 = 403,
        Tutorial5 = 404,
        Tutorial6 = 405,
        Tutorial7 = 406,
    }
}
