using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial1 : MonoBehaviour {

    public string[] TutorialText;
    public int index;
    public TextMeshProUGUI InGameText;
    public GameManager gameManager;
	// Use this for initialization
	void Awake () {
        InGameText.text = TutorialText[0];
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.N))
        {
            index++;
            InGameText.text = TutorialText[index];
            gameManager.ChangeBehaviour();
        }
        if (gameManager.currentPhase == PhaseBuilder.PhaseType.Tutorial6)
        {
            index++;
            InGameText.text = TutorialText[index];
            gameManager.currentPhase = PhaseBuilder.PhaseType.Tutorial7;
        }
	}
}
