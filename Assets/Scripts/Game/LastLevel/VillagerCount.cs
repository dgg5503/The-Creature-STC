using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VillagerCount : MonoBehaviour {
    public int villagerCount;
    public Text countText;

	// Use this for initialization
	void Start () {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; ++i)
            enemies[i].deathCallback += ChangeCount;

        villagerCount = enemies.Length;
        countText = GetComponent<Text>();
        countText.text = villagerCount.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        countText.text = villagerCount.ToString();
	}
    void ChangeCount()
    {
        villagerCount -= 1;
    }
}
