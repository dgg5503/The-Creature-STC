using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    private Button resumeButton;
    private Button mainMenuButton;
    private bool alphaChange = false;
    private Image imageToChange;
    private Text paused;
    private Color alphaColor;
    private Color alphaColorResumeButton;
    private Color alphaColorBackToMenu;
    private Color alphaForPaused;
    private float timeElapsed = 0.0f;
    private float timeElapsedFromResume = 0.0f;
    private bool startCount = false;
    private bool escapeButtonPressed = false;
    private bool resumeButtonPressed = false;
    private Canvas pauseScreen;
    private bool canPressButton = true;
	// Use this for initialization
	void Start () {
        
        resumeButton = GameObject.Find("Resume").GetComponent<Button>();
        mainMenuButton = GameObject.Find("Back To Main Menu").GetComponent<Button>();
        imageToChange = GameObject.Find("PausedImage").GetComponent<Image>();
        paused = GameObject.Find("PausedMenuText").GetComponent<Text>();
        pauseScreen = GameObject.Find("PauseScreen").GetComponent<Canvas>();
        alphaColor = imageToChange.color;
        alphaColor.a = 0.0f;
        imageToChange.color = alphaColor;

        alphaColorResumeButton = resumeButton.GetComponent<Text>().color;
        alphaColorResumeButton.a = 0.0f;
        resumeButton.GetComponent<Text>().color = alphaColorResumeButton;


        alphaColorBackToMenu = mainMenuButton.GetComponent<Text>().color;
        alphaColorBackToMenu.a = 0.0f;
        mainMenuButton.GetComponent<Text>().color = alphaColorBackToMenu;

        alphaForPaused = paused.color;
        alphaForPaused.a = 0.0f;
        paused.color = alphaForPaused;
        pauseScreen.enabled = false;

        

	}
	
	// Update is called once per frame
    void Update()
    {
        if (canPressButton == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escapeButtonPressed = true;
                pauseScreen.enabled = true;
                canPressButton = false;
            }
        }


        if (escapeButtonPressed == true)
        {
            fadeIn();
        }

        if (resumeButtonPressed == true)
        {
            fadeOut();
        }
        


    }

    public void resumeAction() {
        resumeButtonPressed = true;
        Time.timeScale = 1;
    }

    public void fadeIn()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 0.05)
        {
            if (alphaColor.a < 1)
            {
                alphaColor.a += 0.1f;
                imageToChange.color = alphaColor;

                alphaColorResumeButton.a += 0.1f;
                resumeButton.GetComponent<Text>().color = alphaColorResumeButton;

                alphaColorBackToMenu.a += 0.1f;
                mainMenuButton.GetComponent<Text>().color = alphaColorBackToMenu;

                alphaForPaused.a += 0.1f;
                paused.color = alphaForPaused;
                Debug.Log("AlphaREsume:" + alphaColorResumeButton.a);

            }
            else
            {
                escapeButtonPressed = false;
                Time.timeScale = 0;
            }
            timeElapsed = 0;
        }
        Debug.Log("I'm FadingIn");
    }

    public void fadeOut()
    {
        timeElapsedFromResume += Time.deltaTime;
        if (timeElapsedFromResume > 0.05)
        {
            if (alphaColor.a > 0)
            {
                alphaColor.a -= 0.1f;
                imageToChange.color = alphaColor;

                alphaColorResumeButton.a -= 0.1f;
                resumeButton.GetComponent<Text>().color = alphaColorResumeButton;

                alphaColorBackToMenu.a -= 0.1f;
                mainMenuButton.GetComponent<Text>().color = alphaColorBackToMenu;

                alphaForPaused.a -= 0.1f;
                paused.color = alphaForPaused;
                Debug.Log("I'm Called Right Here " + alphaForPaused.a);
            }
            else
            {
                resumeButtonPressed = false;
                canPressButton = true;
                pauseScreen.enabled = false;
            }
            timeElapsedFromResume = 0;
        }
    }

    public void backToMainMenu() {
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("The_Creature"));
        Destroy(GameObject.Find("Canvas"));
        Destroy(GameObject.Find("FreeLookCameraRig"));
        SceneManager.LoadScene("UI Main Menu");
    }



}
