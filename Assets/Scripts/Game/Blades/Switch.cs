using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Switch : MonoBehaviour {

    public int switchNumber;
    GameObject wallManager;
    private int counter;
    // Use this for initialization
    void Start () {
        wallManager = GameObject.Find("WallManagerObj");
    }
	
	// Update is called once per frame
	void Update () {
        counter++;
        if(counter >= 300)
        {
            GameObject.Find("LevelPulledText").GetComponent<Text>().text = "";
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
            GameObject.Find("LevelPulledText").GetComponent<Text>().text = "Level Pulled/n Plates Values Switch";
        }
    }
}
