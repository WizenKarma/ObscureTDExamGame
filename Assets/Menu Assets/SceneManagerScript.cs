using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {

	void Update () 
	{
		/*if (Input.GetKey(KeyCode.Escape))
		{
            Cursor.visible = true;
            SceneManager.LoadScene("Menu");
		}*/
	}

    public void Menu()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    public void LevelOne()
	{
        Cursor.visible = true;
        SceneManager.LoadScene("Beta1");
	}

    public void LevelTwo()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Beta2");
    }

    public void Controls()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Controls");
    }

    public void Credits()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Credits");
    }
    /*
    public void QuitGame()
	{
		Application.Quit();
	}
    */
}
