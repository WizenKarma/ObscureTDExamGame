using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct NumberOfTowers{
    public Tower Tower;
    [Range(1,5)]
    public int Number;
}

[CreateAssetMenu]
public class Combiner : ScriptableObject {
    public List<NumberOfTowers> components;
    public List<NumberOfTowers> output;

    public bool CanCraft(iTowerController towerContainer) {
        foreach (NumberOfTowers number in components) {
            if (towerContainer.TowerCount(number.Tower.ID) < number.Number)
                return false;
        }
        return true;
    }

    public void Craft(iTowerController towerContainer) {
        if (CanCraft(towerContainer)) {
            foreach (NumberOfTowers number in components) {
                for (int i = 0; i < number.Number; i++)
                {
                    //towerContainer.RemoveTower(number.Tower.ID);
                    Debug.Log(towerContainer.KillTower(number.Tower.ID).name);
                }
            }

            foreach(NumberOfTowers number in output) {
                for (int i = 0; i < number.Number; i++)
                {   
                    towerContainer.AddTower(number.Tower);
                }
            }
        }
    }


}
