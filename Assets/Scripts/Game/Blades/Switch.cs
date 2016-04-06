using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

    public int switchNumber;
    GameObject wallManager;
    // Use this for initialization
    void Start () {
        wallManager = GameObject.Find("WallManagerObj");
    }
	
	// Update is called once per frame
	void Update () {
	    
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
        }
    }
}
