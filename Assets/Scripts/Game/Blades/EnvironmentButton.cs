using UnityEngine;
using System.Collections;

public class EnvironmentButton : MonoBehaviour {

    GameObject wallManager;
    public int buttonNumber;
    
	// Use this for initialization
	void Start () {
        wallManager = GameObject.Find("WallManagerObj");
    }
	
	// Update is called once per frame
	void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Debug.Log("Lift floor");
            wallManager.GetComponent<WallManager>().lastButtonTouched = buttonNumber;
            wallManager.GetComponent<WallManager>().ManageButtons(buttonNumber);
        }
    }

    public void Activate(GameObject[] wallArray)
    {
        for (int i = 0; i < wallArray.Length; i++)
        {
            MoveBlade temp = wallArray[i].GetComponent<MoveBlade>(); //having trouble changin the variable through temp... always keeps on being true;
            temp.MoveUp = false;  
        }
    }

    public void UnActivate(GameObject[] wallArray)
    {
        //Debug.Log(wallArray.Length);
        for (int i = 0; i < wallArray.Length; i++)
        {
            MoveBlade temp = wallArray[i].GetComponent<MoveBlade>();
            temp.MoveUp = true;
            //Debug.Log(wallArray[i].name);
        }
    }
}
