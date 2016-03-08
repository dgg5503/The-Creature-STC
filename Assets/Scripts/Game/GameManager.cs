using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 9);
        // Have bparts ignore character colliders
        Physics.IgnoreLayerCollision(9, 10);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
