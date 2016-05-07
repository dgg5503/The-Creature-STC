using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayPoint : MonoBehaviour {

    public List<GameObject> wayPoints = new List<GameObject>();

    void Awake() {
    }
    
    // Use this for initialization
	void Start () {
      
	}
	
	// Update is called once per frame
	void Update () {
        foreach (GameObject wayPointToGo in wayPoints)
        {
            
            moveCamera(wayPointToGo.GetComponent<Transform>().position);
        }


	}

    public void moveCamera(Vector3 moveToPosition) {
        this.transform.position = Vector3.MoveTowards(this.transform.position, moveToPosition, 2 * Time.deltaTime); 
    }
}
