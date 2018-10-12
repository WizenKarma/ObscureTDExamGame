using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDisplay : MonoBehaviour
{


    List<Combiner> recipes = new List<Combiner>();
    List<PlayerControllerScript.Possible> towers = new List<PlayerControllerScript.Possible>();

    public PlayerControllerScript player;
    public GameObject IndividualTowerDisplay;
    public Canvas displayParent;
    public RectTransform ButtonParent;
    public Button TowerListButton;
    // Use this for initialization
    void Start()
    {
        recipes = player.possibleRecipes();
        towers = player.possibleTowers();

        int entries = recipes.Count + towers.Count;
        ButtonParent.sizeDelta = new Vector2(ButtonParent.sizeDelta.x, entries * TowerListButton.GetComponent<RectTransform>().sizeDelta.y + entries * ButtonParent.GetComponent<VerticalLayoutGroup>().padding.top);
        int indexer = 0;
        foreach (PlayerControllerScript.Possible p in towers)
        {
            Button b = Instantiate(TowerListButton, ButtonParent);
            b.onClick.AddListener(delegate { TowerClicked(indexer,ButtonParent.gameObject); });
            indexer++;
        }
        indexer = 0;
        foreach (Combiner c in recipes)
        {
            Button b = Instantiate(TowerListButton, ButtonParent);
            b.onClick.AddListener(delegate { CombinationClicked(indexer, ButtonParent.gameObject); });
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
    }

    public void CombinationClicked(int index, GameObject DisplayToHide) //for when a combination is clicked
    {
        DisplayToHide.SetActive(false);
        GameObject g = Instantiate(IndividualTowerDisplay, displayParent.transform);
        Button b = g.GetComponentInChildren<Button>();
        b.onClick.AddListener(delegate { BackToMainDisplay(g, DisplayToHide); });

        RectTransform[] displayPieces = g.GetComponentsInChildren<RectTransform>();
        displayPieces[1].GetComponent<Image>().sprite = recipes[index].output[0].Tower.preview;
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
