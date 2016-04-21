using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VillagerCount : MonoBehaviour {
    public int villagerCount;
    private int lastCount;
    public Text countText;

	// Use this for initialization
	void Start () {
        villagerCount = GameObject.FindGameObjectsWithTag("Villager").Length;
        lastCount = villagerCount; 
	}
	
	// Update is called once per frame
	void Update () {
	    if(lastCount != villagerCount)
        {
            
            countText.text = villagerCount.ToString();
            lastCount = villagerCount;
            
            
        }
	}
}
