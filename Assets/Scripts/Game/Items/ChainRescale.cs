using UnityEngine;
using System.Collections;

public class ChainRescale : MonoBehaviour {
    private GameObject hook;
    private GameObject grapplingPart;

	// Use this for initialization
	void Start () {
        hook = GameObject.Find("HookPoint");
        grapplingPart = GameObject.Find("GrapperPoint");
	}
	
	// Update is called once per frame
	void Update () {
        float scaleX = Vector3.Distance(hook.transform.position, grapplingPart.transform.position);
        GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(scaleX, 1f);
	}
}
