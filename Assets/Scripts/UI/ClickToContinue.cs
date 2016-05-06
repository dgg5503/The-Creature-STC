using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickToContinue : MonoBehaviour {
    private float timeCounter;
    Text clickText;
    private bool showOnce;
    public string SceneLevel;

    void Start () {
        timeCounter = 0.0f;
        clickText = GameObject.Find("ClickText").GetComponent<Text>();
        showOnce = false;
    }
	
	void Update () {
        timeCounter += Time.deltaTime;
        if(timeCounter >= 0.1f && showOnce == false)
        {
            showOnce = true;
            clickText.text += "Click 'P' to continue";
        }
        if(showOnce == true)
        {
            if (Input.GetKeyDown("p"))
            {
                DontDestroyOnLoad(GameObject.Find("The_Creature"));
                GameObject canv = GameObject.Find("Canvas");
                DontDestroyOnLoad(canv);
                canv.SetActive(true);
                DontDestroyOnLoad(GameObject.Find("FreeLookCameraRig"));
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneLevel);
            }
        }
	}
}
