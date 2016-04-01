﻿/*
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
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        accelerationScalar = 0;
        rotationAccelFactor = 0;
        maxSpeed = 0;
        IsAlive = false;

        // tag as corpse for easy mouse detection
        tag = "Corpse";
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    { 
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Detach(3);
        }

        // TODO: APPEND CAPSULE COLLIDER TO TORSO?
        transform.position = root.transform.position;
        transform.rotation = root.transform.rotation;
    }

    /// <summary>
    /// Corpses dont move...
    /// </summary>
    protected override void ProcessMovement() { }
}
