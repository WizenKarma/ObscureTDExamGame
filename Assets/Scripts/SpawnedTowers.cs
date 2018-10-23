using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnedTowers: MonoBehaviour, iTowerController {

    [Serializable]
    public struct TowerSet
    {
        public Tower tower;
        public GameObject realTower;
    }

    [SerializeField] List<TowerSet> spawnedTowers;
    [SerializeField] Transform towerParent;
    [SerializeField] GameObject wall;

    private void Awake()
    {
        spawnedTowers = new List<TowerSet>();
    }

    public int CountTowers() {
        return spawnedTowers.Count;
    }

    public bool AddTower(Tower tower, GameObject towerObj) {
        if (IsFull())
            return false;
        Tower t = Instantiate(tower);
        TowerSet towerToUse = new TowerSet();
        towerToUse.tower = t;
        towerToUse.realTower = towerObj;
        spawnedTowers.Add(towerToUse);
        return true;
    }

    public bool AddTower(Tower tower)
    {
        if (IsFull())
            return false;
        Tower t = Instantiate(tower);
        TowerSet towerToUse = new TowerSet();
        towerToUse.tower = t;
        //towerToUse.realTower = towerObj;
        spawnedTowers.Add(towerToUse);
        return true;
    }

    public bool ContainsTower(Tower tower)
    {
        for (int i = 0; i < spawnedTowers.Count; i++)
        {
            if (spawnedTowers[i].tower == tower)
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsThisTower(Tower tower)
    {

        for (int i = 0; i < spawnedTowers.Count; i++)
        {
            if (spawnedTowers[i].tower.TargetTower == tower.TargetTower)
            {
                return true;
            }
        }
        return false;
    }

    public List<Tower> GetAllTowers() {
        List<Tower> temp = new List<Tower>();
        foreach (TowerSet ts in spawnedTowers)
        {
            temp.Add(ts.tower);
        }
        return temp;
    }


    public bool Keep() {
        Destroy(spawnedTowers[0].tower);
        return true;
    }

    public bool IsFull()
    {
        return false;
    }

    /*public bool RemoveTower(Tower tower) { shouldnt be necessary
        if (spawnedTowers.Remove(tower)) {
            Destroy(tower);
            return true;
        }
        return false;
    }*/

    public List<TowerSet> SaveTower(GameObject towerToSave)
    {
        for (int i = spawnedTowers.Count - 1; i >= 0; i--)
        {
            TowerSet tower = spawnedTowers[i];
            if (towerToSave != null && towerToSave != spawnedTowers[i].realTower)
            {
                //Instantiate(wall, spawnedTowers[i].realTower.transform.position, Quaternion.identity);
                if(spawnedTowers[i].realTower != null)
                    Destroy(spawnedTowers[i].realTower.gameObject);
                spawnedTowers.Remove(spawnedTowers[i]);
            }
        }
        return spawnedTowers;
    }

    public bool clearList()
    {
        spawnedTowers = new List<TowerSet>();
        return true;
    }
    public bool AddTowers(List<TowerSet> towers)
    {
        spawnedTowers.AddRange(towers);
        return true;
    }

    public Tower RemoveTower(string towerID)
    {
        for (int i = 0; i < spawnedTowers.Count; i++)
        {
            Tower tower = spawnedTowers[i].tower;
            if (tower != null && tower.ID == towerID)
            {
                spawnedTowers.Remove(spawnedTowers[i]);//spawnedTowers[i] = null;
                return tower;
            }
        }
        return null;
    }

    public bool RemoveTower(Tower tower)
    {
        throw new NotImplementedException();
    }

    public Tower KillTower(string towerID)
    {
       for (int i = 0; i < spawnedTowers.Count; i++)
        {
            Tower tower = spawnedTowers[i].tower;
            if (tower != null && tower.ID == towerID)
            {
                spawnedTowers.Remove(spawnedTowers[i]);//spawnedTowers[i] = null;
                Destroy(tower.TargetTower.gameObject);
                return tower;
            }
        }
        return null;
    }

    public void InsertAtFirst(GameObject towerObj) {
        foreach(TowerSet t in spawnedTowers)
        {
            if (t.realTower.gameObject == towerObj.gameObject)
            {
                spawnedTowers.Remove(t);
                spawnedTowers.Insert(0, t);
                return;
            }
        }
    }

    public int TowerCount(string towerID)
    {
        int count = 0;
        for (int i = 0; i < spawnedTowers.Count; i++)
        {
            if (spawnedTowers[i].tower.ID == towerID)
            {
                count++;
            }
        }
        return count;
    }

 
    public bool AddTower()
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        spawnedTowers.Clear();
    }
}
