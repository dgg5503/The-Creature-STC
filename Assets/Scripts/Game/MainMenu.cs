using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

    private Canvas QuitMenu;
    private Button NewGame;
    private Button Exit;
    private Button Settings;
	// Use this for initialization
	void Start () {
        QuitMenu = GameObject.Find("QuitMenu").GetComponent<Canvas>();
        NewGame = GameObject.Find("NewGame").GetComponent<Button>();
        Exit = GameObject.Find("Exit").GetComponent<Button>();
        Settings = GameObject.Find("Settings").GetComponent<Button>();
        Settings.interactable = false;
        QuitMenu.enabled = false; 

	}
	
	// Update is called once per frame
	void Update () {
	    
	}


    public void exitPressed()
    {
        QuitMenu.enabled = true;
        Exit.enabled = false;
        NewGame.enabled = false;

    }

    public void newGamePressed()
    {
        SceneManager.LoadScene(1);
    }

    public void noOption()
    {
        QuitMenu.enabled = false;
        NewGame.enabled = true;
        Exit.enabled = true;
    }

    public void yesOption()
    {
        Application.Quit();
    }
}
