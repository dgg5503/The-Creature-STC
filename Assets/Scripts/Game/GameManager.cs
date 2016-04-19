using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Joint name dicitionary
    // Will be expanded if new joints are added
    //[SerializeField]
    public static Dictionary<int, string> jointTypeDict;
    
    /// <summary>
    /// Get the layer mask used for checking whether or not a character is grounded
    /// </summary>
    public static LayerMask GroundedLayerMask { get; private set; }

    void Awake()
    { 
        // Have bparts ignore character colliders
        Physics.IgnoreLayerCollision(9, 10);

        // http://answers.unity3d.com/questions/416919/making-raycast-ignore-multiple-layers.html
        // ignore layers 9 and 10 as they are not considered to be "ground"
        GroundedLayerMask = ~((1 << 9) | (1 << 10));

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
