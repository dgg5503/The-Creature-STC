using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    void Awake()
    {
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
