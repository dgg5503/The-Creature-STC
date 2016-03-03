/*
    Corpse Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - Used for objects in the scene that are spawned as dead and will never
          ever move.
*/

using UnityEngine;
using System.Collections;

public class Corpse : Character
{

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected override void Awake()
    {
        // Initiate parent first THEN move on to this classes stuff.
        base.Awake();

        // 0 everything out
        accelerationScalar = 0;
        rotationAccelFactor = 0;
        maxSpeed = 0;
        isAlive = false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    /// <summary>
    /// Corpses dont move...
    /// </summary>
    protected override void ProcessMovement() { }
}
