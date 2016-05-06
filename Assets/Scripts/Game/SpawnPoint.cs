using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject creature = GameObject.Find("The_Creature");
        GameObject camera = GameObject.Find("FreeLookCameraRig");
        GameObject cameraSpawn = GameObject.Find("CameraSpawn");
        GameObject playerSpawn = GameObject.Find("PlayerSpawn");

        creature.transform.position = playerSpawn.transform.position;
        camera.transform.position = cameraSpawn.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
