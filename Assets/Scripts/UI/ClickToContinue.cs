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
        if(timeCounter >= 2.0f && showOnce == false)
        {
            showOnce = true;
            clickText.text += "Click 'P' to continue";
        }
        if(showOnce == true)
        {
            if (Input.GetKeyDown("p"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneLevel);
            }
        }
	}
}
