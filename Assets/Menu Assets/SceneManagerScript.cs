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
            SceneManager.LoadScene(1);
		}
	}

    public void Menu()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(1);
    }

    public void StartGame()
	{
        Cursor.visible = true;
        SceneManager.LoadScene(50);
	}

    public void Controls()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(2);
    }

    public void Credits()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(3);
    }

    public void QuitGame()
	{
		Application.Quit();
	}
}
