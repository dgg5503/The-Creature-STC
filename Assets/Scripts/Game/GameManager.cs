using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    void Awake()
    {
        // items ignore collision with anything on the ITEM LAYER
        Physics.IgnoreLayerCollision(9, 9);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
