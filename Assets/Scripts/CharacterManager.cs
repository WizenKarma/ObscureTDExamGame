using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterManager : MonoBehaviour {
    [System.Serializable]
    public struct Possible {
        public Tower tower;
        public int weight;//higher weight means more frequent
        public float occurence;
    }
    public List<Combiner> recipes = new List<Combiner>();
    public List<Possible> towers = new List<Possible>();
    public List<Tower> towersToPlaceThisRound = new List<Tower>();

    public GameObject wall;
    public GameObject towerBase;
    public SpawnedTowers Inventory;
    public SpawnedTowers RoundInventory;
    public Button BuildButtonPrefab;
    public RectTransform BuildButtonParent;

    float totalWeight;

    private void OnValidate()
    {
        totalWeight = 0;
        foreach (Possible t in towers)
            totalWeight += t.weight;
        Inventory = this.GetComponent<SpawnedTowers>();
    }

    private void Awake()
    {
        OnValidate();
    }

    public void destroy(GameObject towerToSave)
    {
        if(!Inventory.AddTowers(RoundInventory.SaveTower(towerToSave))){
            print("there has been an error when attempting to add the temporary inventory to the permanent one in CharacterManager.cs");
            return; 
        };
        RoundInventory.clearList();
    }

    public void PopulateBuildButtons(int index) {
        // Generate a random position in the list.
        float pick = Random.value * totalWeight;
        int chosenIndex = 0;
        float cumulativeWeight = towers[0].weight;
        // Step through the list until we've accumulated more weight than this.
        // The length check is for safety in case rounding errors accumulate.
        while (pick > cumulativeWeight && chosenIndex < towers.Count - 1)
        {
            chosenIndex++;
            cumulativeWeight += towers[chosenIndex].weight;
        }
        Button btn = Instantiate(BuildButtonPrefab, BuildButtonParent);
        btn.GetComponentInChildren<Image>().sprite = towers[chosenIndex].tower.preview;
        this.GetComponent<PlayerControllerScript>().setBuildTowers(towers[chosenIndex].tower, index);
    }

    public void Keep() {
        if (RoundInventory.Keep())
            return;
        else
            print("big problems in Character manager, tower could not be kept");
    }

    public void getReadyToBuild() {
        for (int i = 0; i < 5; i++)
        {
            PopulateBuildButtons(i); //this just populates 5 buttons, doesnt actually build anything yet.
        }
    }

    // Use this for initialization
    public void build(Vector3 pos, Tower towerToBuild) {
        /*
           GameObject tower =  Instantiate(towers[chosenIndex].tower.prefab, pos, Quaternion.identity); //where the tower is instantiated
           Tower SOTower = Instantiate(towers[chosenIndex].tower);
           tower.GetComponent<InGameTower>().tower = SOTower;
           RoundInventory.AddTower(tower.GetComponent<InGameTower>().tower, tower);
           print("Built a: " + towers[chosenIndex].tower.name + " with a " + towers[chosenIndex].weight + " in " + totalWeight + " chance.");
       */
        GameObject tow = Instantiate(towerToBuild.prefab, pos + Vector3.up, Quaternion.identity); //where the tower is instantiated
        Instantiate(wall, pos + Vector3.up * 0.5f, Quaternion.identity);
        Tower SOTower = Instantiate(towerToBuild);
        tow.GetComponent<InGameTower>().tower = SOTower;
        RoundInventory.AddTower(tow.GetComponent<InGameTower>().tower, tow);
        print("attempted to build a tower");
    }
	
}
