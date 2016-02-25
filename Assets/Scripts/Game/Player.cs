using UnityEngine;
using System.Collections;

public class Player : Character {
    int i;
    // Use this for initialization
    void Start () {
	    
	}

	// Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0))
        {
            for (i = 0; i < bodyParts.Count && Detach(i) == null; i++) ;
        }
    }
}
