using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial3 : MonoBehaviour {

    public string[] TutorialText;
    public int index;
    public TextMeshProUGUI InGameText;
    public GameManager gameManager;
    public bool nextTut;
	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.N))
        {
            switch(index)
            {
                case 0:
                    index++;
                    InGameText.text = TutorialText[index];
                    gameManager.ChangeBehaviour();
                    break;
                case 1:
                    if (nextTut)
                    {
                        index++;
                        InGameText.text = TutorialText[index];
                        gameManager.ChangeBehaviour();
                    }
                    break;
                 default:
                    index++;
                    InGameText.text = TutorialText[index];
                    gameManager.ChangeBehaviour();

                    break;

            }
        }
        if (gameManager.currentPhase == PhaseBuilder.PhaseType.Tutorial6)
        {
            index++;
            InGameText.text = TutorialText[index];
            gameManager.currentPhase = PhaseBuilder.PhaseType.Tutorial7;
        }
	}
}
