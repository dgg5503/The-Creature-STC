using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if (transform.GetComponent<Image>().fillAmount > 0.6)
        {
            transform.GetComponent<Image>().color = Color.green;
        }
        else if (transform.GetComponent<Image>().fillAmount <= 0.6 && transform.GetComponent<Image>().fillAmount >= 0.4)
        {
            transform.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            transform.GetComponent<Image>().color = Color.red;
        }
            
	}
}
