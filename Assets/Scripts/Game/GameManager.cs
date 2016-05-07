using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameState
{
    MainMenu,
    InGame,
    Pause
}

/// <summary>
/// This must be in EVERY SCENE otherwise collision and isGrounded wont work!
/// </summary>
public class GameManager : MonoBehaviour
{
    private static Dictionary<string, Object> prefabDictionary;

    /// <summary>
    /// Get the layer mask used for checking whether or not a character is grounded
    /// </summary>
    public static LayerMask GroundedLayerMask { get; private set; }
    
    /// <summary>
    /// Get the layer mask that will ignore body parts.
    /// </summary>
    public static LayerMask BodyPartLayerMask { get; private set; }

    /// <summary>
    /// Get the layer mask that ignores all colliders except characters.
    /// </summary>
    public static LayerMask CharacterLayerMask { get; private set; }

    public static LayerMask RaycastLayerMask { get; private set; }

    /// <summary>
    /// Get or set the current game state.
    /// </summary>
    public static GameState GameState { get; set; }

    public static InputManager InputManager { get; private set; }

    /// <summary>
    /// Get all prefabs from the game.
    /// </summary>
    public static Dictionary<string, Object> PrefabDictionary
    {
        get
        {
            return prefabDictionary;
        }
    }

    void Awake()
    {
        // start in MM
        // TODO: CHANGE THIS WHEN FULL GAME IMPLEMENTED
        GameState = GameState.InGame;

        // Have bparts ignore character colliders
        Physics.IgnoreLayerCollision(9, 10);
        Physics.IgnoreLayerCollision(9, 11);
        Physics.IgnoreLayerCollision(2, 9);
        Physics.IgnoreLayerCollision(8, 8);
        Physics.IgnoreLayerCollision(9, 8);

        // http://answers.unity3d.com/questions/416919/making-raycast-ignore-multiple-layers.html
        // ignore layers 9 and 10 as they are not considered to be "ground"
        GroundedLayerMask = ~((1 << 9) | (1 << 10) | (1 << 11) | (1 << 2));
        BodyPartLayerMask = ~((1 << 9) | (1 << 11));
        CharacterLayerMask = (1 << 10);
        RaycastLayerMask = ~(1 << 2);
        // load all prefabs!
        prefabDictionary = Resources.LoadAll("Prefabs/Items").ToDictionary(x => x.name, x => x);

        // DEBUG LOOK
        /*
        foreach (KeyValuePair<string, Object> kvp in prefabDictionary)
            Debug.Log(kvp.Key);
        */
        InputManager = GetComponent<InputManager>();
    }
}
