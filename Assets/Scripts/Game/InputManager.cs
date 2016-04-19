using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    public delegate void KeyAction();
    public event KeyAction keyAction;
    private Dictionary<KeyCode, KeyAction> keyBinds;
    KeyCode[] boundKeys = null;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        keyBinds.Keys.CopyTo(boundKeys, 0);
        for (int i = 0; i < boundKeys.Length; ++i)
            if (Input.GetKey(boundKeys[i]))
                keyBinds[boundKeys[i]]();
	}

    public void BindKey(KeyCode keyCode, KeyAction keyAction)
    {
        // see if key already exists
    }
}
