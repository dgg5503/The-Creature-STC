using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {
    bool isDestroyed;

    public bool IsDestroyed
    {
        get
        {
            return isDestroyed;
        }
        set
        {
            isDestroyed = value;
        }
    }
	// Use this for initialization
	void Start () {
        isDestroyed = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if(isDestroyed == true)
        {

        }
	}
}
