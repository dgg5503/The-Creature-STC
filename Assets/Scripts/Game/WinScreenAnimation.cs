using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreenAnimation : MonoBehaviour {

    private Transform victoryImage;
    private float countInt = 0.0f;
    private bool finishedAnimation = false;
	// Use this for initialization
	void Start () {
        victoryImage = GameObject.Find("VictoryImage").GetComponent<Transform>();
        victoryImage.localScale = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
	}


    public void playAnimation() {
        if (finishedAnimation == false)
        {
            countInt += 0.1f;
            if (countInt > 0.5)
            {
                victoryImage.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                countInt = 0;
            }
            if (victoryImage.localScale == new Vector3(1, 1, 1))
            {
                finishedAnimation = true;
            }
        }

    }
}
