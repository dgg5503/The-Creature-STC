/*
    InputManager
    ------------------------
    AUTHS Douglas Gliner
    
*/

using UnityEngine;
using System.Collections.Generic;

public enum KeyState
{
    KEY_DOWN,
    KEY_UP,
    KEY_PRESSED
}

public class InputManager : MonoBehaviour {
    // Fields
    /// <summary>
    /// The function pointer signature called appropriately.
    /// </summary>
    public delegate void KeyAction(KeyState keyState);
    private Dictionary<KeyCode, Dictionary<GameState, KeyAction>> keyBinds;
    private Dictionary<KeyCode, Dictionary<GameState, KeyAction>>.KeyCollection boundKeys;

    void Awake()
    {
        // init key binds
        keyBinds = new Dictionary<KeyCode, Dictionary<GameState, KeyAction>>();
    }

    // Update is called once per frame
    void Update() {
        boundKeys = keyBinds.Keys;
        foreach (KeyCode keyCheck in boundKeys)
            if (keyBinds.ContainsKey(keyCheck) &&
                keyBinds[keyCheck].ContainsKey(GameManager.GameState))
            {
                if (Input.GetKeyDown(keyCheck))
                    keyBinds[keyCheck][GameManager.GameState](KeyState.KEY_DOWN);
                else if (Input.GetKeyUp(keyCheck))
                    keyBinds[keyCheck][GameManager.GameState](KeyState.KEY_UP);
                else if (Input.GetKey(keyCheck))
                    keyBinds[keyCheck][GameManager.GameState](KeyState.KEY_PRESSED);
            }
	}

    /// <summary>
    /// Binds a keycode to a given action. Multiple actions can be bound to the same keyAction.
    /// </summary>
    /// <param name="keyCode">The key code to bind.</param>
    /// <param name="keyAction">Action to execute when key code hit.</param>
    public void BindKey(KeyAction keyAction, KeyCode keyCode, GameState gameState = GameState.InGame)
    {
        // see if key already exits
        if(keyBinds.ContainsKey(keyCode))
        {
            // see if game state exists
            if(keyBinds[keyCode].ContainsKey(gameState))
            {
                // add it
                keyBinds[keyCode][gameState] += keyAction;
            }
            else
            {
                // make it
                keyBinds[keyCode][gameState] = keyAction;
            }
        }
        else
        {
            // initialize inner dictionary and add
            keyBinds[keyCode] = new Dictionary<GameState, KeyAction>();
            keyBinds[keyCode][gameState] = keyAction;
        }
    }
}
