using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Switch : MonoBehaviour {

    public int switchNumber;
    GameObject wallManager;
    private int counter;
    private bool goTimer = false;
    // Use this for initialization
    void Start () {
        wallManager = GameObject.Find("WallManagerObj");
    }

    // Update is called once per frame
    void Update() {
        if (goTimer == true) { 
            counter++;
        }

        if(counter >= 300)
        {
            GameObject.Find("LevelPulledText").GetComponent<Text>().text = "";
            goTimer = false;
            counter = 0;
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("Switched Acivated");
            wallManager.GetComponent<WallManager>().onSwitch = switchNumber;
            Debug.Log("Last Button Touched: " + wallManager.GetComponent<WallManager>().lastButtonTouched);
            wallManager.GetComponent<WallManager>().ResetWalls();
            wallManager.GetComponent<WallManager>().ManageButtons(wallManager.GetComponent<WallManager>().lastButtonTouched);
            GameObject.Find("LevelPulledText").GetComponent<Text>().text = "Level Pulled Plates Values Switch";
            goTimer = true;
        }
    }
}
