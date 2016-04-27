using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    // Fields
    /// <summary>
    /// The function pointer signature called appropriately.
    /// </summary>
    public delegate void KeyAction();
    private Dictionary<KeyCode, Dictionary<GameState, KeyAction>> keyBinds;
    private Dictionary<KeyCode, Dictionary<GameState, KeyAction>>.KeyCollection boundKeys;

    void Awake()
    {
        // init key binds
        keyBinds = new Dictionary<KeyCode, Dictionary<GameState, KeyAction>>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        boundKeys = keyBinds.Keys;
        foreach (KeyCode keyCheck in boundKeys)
            if (Input.GetKey(keyCheck) &&
                keyBinds.ContainsKey(keyCheck) &&
                keyBinds[keyCheck].ContainsKey(GameManager.GameState))
                keyBinds[keyCheck][GameManager.GameState]();
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
