using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeDisplay : MonoBehaviour
{


    List<Combiner> recipes = new List<Combiner>();
    List<PlayerControllerScript.Possible> towers = new List<PlayerControllerScript.Possible>();

    public PlayerControllerScript player;
    public GameObject IndividualTowerDisplay;
    public Canvas displayParent;
    public RectTransform ButtonParent;
    public Button TowerListButton;
    public GameObject spacer;

    private int totalWeight;

    // Use this for initialization
    void Start()
    {

        
        recipes = player.possibleRecipes();
        towers = player.possibleTowers();

        foreach (PlayerControllerScript.Possible t in towers)
            totalWeight += t.weight;

        int entries = recipes.Count + towers.Count;
        ButtonParent.sizeDelta = new Vector2(ButtonParent.sizeDelta.x, entries * TowerListButton.GetComponent<RectTransform>().sizeDelta.y + entries * ButtonParent.GetComponent<VerticalLayoutGroup>().padding.top * 1.5f);
        int indexer = 0;
        foreach (PlayerControllerScript.Possible p in towers)
        {
            int i = indexer;//dirty fix for a strange memory grab that it keeps doing
            Button b = Instantiate(TowerListButton, ButtonParent);
            b.onClick.AddListener(delegate { TowerClicked(i,ButtonParent.gameObject); });
            b.GetComponentInChildren<Text>().text = p.tower.name;
            indexer++;
        }

        Instantiate(spacer, ButtonParent);
        indexer = 0;
   
        foreach (Combiner c in recipes)
        {
            int i = indexer;
            Button b = Instantiate(TowerListButton, ButtonParent);
            b.onClick.AddListener(delegate { CombinationClicked(i, ButtonParent.gameObject); });
            b.GetComponentInChildren<Text>().text = c.output[0].Tower.name;
            indexer++;
        }
    }

    public void TowerClicked(int index, GameObject DisplayToHide) //for when a tower is clicked
    {
        DisplayToHide.SetActive(false);
        GameObject g = Instantiate(IndividualTowerDisplay, displayParent.transform);
        Button b = g.GetComponentInChildren<Button>();
        b.onClick.AddListener(delegate { BackToMainDisplay(g,DisplayToHide); });

        RectTransform[] displayPieces = g.GetComponentsInChildren<RectTransform>();
        displayPieces[1].GetComponent<Image>().sprite = towers[index].tower.preview;
        displayPieces[2].GetComponent<TextMeshProUGUI>().text = towers[index].tower.Description;

        string subText = decimal.Round( (decimal)((float)towers[index].weight / (float)totalWeight * 100f),2) + "";
        displayPieces[3].GetComponent<TextMeshProUGUI>().text = "Chance of appearing: " + subText + "%";
    }

    public void CombinationClicked(int index, GameObject DisplayToHide) //for when a combination is clicked
    {
        print(index);
        DisplayToHide.SetActive(false);
        GameObject g = Instantiate(IndividualTowerDisplay, displayParent.transform);
        Button b = g.GetComponentInChildren<Button>();
        b.onClick.AddListener(delegate { BackToMainDisplay(g, DisplayToHide); });

        RectTransform[] displayPieces = g.GetComponentsInChildren<RectTransform>();
        displayPieces[1].GetComponent<Image>().sprite = recipes[index].output[0].Tower.preview;
        displayPieces[2].GetComponent<TextMeshProUGUI>().text = recipes[index].output[0].Tower.Description;

        string subText = "Components: ";
        for (int i = 0; i < recipes[index].components.Count; i++)
        {
            subText += recipes[index].components[i].Tower.TowerName;
            if (i != recipes[index].components.Count - 1)
            {
                subText += " + ";
            }
        }

        displayPieces[3].GetComponent<TextMeshProUGUI>().text = subText;
    }

    public void BackToMainDisplay(GameObject DisplayToDestroy, GameObject DisplayToShow)
    {
        Destroy(DisplayToDestroy);
        DisplayToShow.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {

    }

}
