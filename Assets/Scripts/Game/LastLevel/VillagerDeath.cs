using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VillagerDeath : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {

       //if(this.gameObject.GetComponent<>)
	}

    void ChangeCount()
    {
        Text counter = GameObject.Find("VillagerCountText").GetComponent<Text>();
        counter.GetComponent<VillagerCount>().villagerCount -= 1;
    }
}
