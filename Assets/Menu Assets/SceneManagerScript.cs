using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {

	void Update () 
	{
		if (Input.GetKey(KeyCode.Escape))
		{
            Cursor.visible = true;
            SceneManager.LoadScene(3);
		}
	}

    public void Menu()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(2);
    }

    public void LevelOne()
	{
        Cursor.visible = true;
        SceneManager.LoadScene(0);
	}

    public void LevelTwo()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(1);
    }

    public void Controls()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(3);
    }

    public void Credits()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(4);
    }

    public void QuitGame()
	{
		Application.Quit();
	}
}
