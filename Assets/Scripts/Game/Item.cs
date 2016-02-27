/*
    Item Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - 
*/

using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour {

    protected virtual void Awake()
    {
        // set layer of items to layer 9
        gameObject.layer = 9;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
