﻿/*
    Player Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - 
*/

using UnityEngine;
using System.Collections;
using System;

public class Player : Character {
    // Camera used by this player
    private Camera playerCamera;


    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected override void Awake()
    {
        // Initiate parent first THEN move on to this classes stuff.
        base.Awake();

        // ??? ASSUMING MAIN CAMERA IS THE CHARACTERS CAMERA
        //playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerCamera = Camera.main;
        accelerationScalar = 10f;
        rotationAccelFactor = 10f;
        maxVelocity = 5.0f;
    }

    // Use this for initialization
    void Start () {
	    
	}

    // Update is called once per frame
    int i;
    protected override void Update () {
        

        if (Input.GetMouseButtonDown(0))
        {
            if (!CheckForAndAttachBodyPart())
                for (i = 0; i < bodyParts.Count && Detach(i) == null; i++) ;
        }

        base.Update();
    }

    protected override void ProcessMovement()
    {
        // Get camera forward
        // OPTIMIZATION: CHARACTER CREATING NEW VEC3 EVERY FRAME?
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        #region movement
        // get direction relative to camera
        if (Input.GetKey(KeyCode.W))
        {
            //acceleration += cameraForward * -1;

            acceleration.x += cameraForward.x;
            acceleration.z += cameraForward.z;
        }

        if (Input.GetKey(KeyCode.A))
        {
            //acceleration += cameraRight * -1;

            acceleration.x += cameraRight.x * -1;
            acceleration.z += cameraRight.z * -1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            //acceleration += cameraForward * -1;

            acceleration.x += cameraForward.x * -1;
            acceleration.z += cameraForward.z * -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            //acceleration += cameraRight;

            acceleration.x += cameraRight.x;
            acceleration.z += cameraRight.z;
        }

        // apply accel scalar AFTER getting direction
        // this is so we dont add two times the accel scalar 
        // when holding down two direction at the same time.
        acceleration *= accelerationScalar;

        // key up
        /*
        if (Input.GetKeyUp(KeyCode.W))
        {
            acceleration.x -= cameraForward.x * accelerationScalar;
            acceleration.z -= cameraForward.z * accelerationScalar;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            //acceleration += cameraRight * -1;

            acceleration.x -= cameraRight.x * -1;
            acceleration.z -= cameraRight.z * -1;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            //acceleration += cameraForward * -1;

            acceleration.x -= cameraForward.x * -1;
            acceleration.z -= cameraForward.z * -1;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            //acceleration += cameraRight;

            acceleration.x -= cameraRight.x;
            acceleration.z -= cameraRight.z;
        }
        */
        #endregion
    }

    // debug function for reattaching body parts.
    bool CheckForAndAttachBodyPart()
    {
        // cast ray from camera to where mouse position is
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast
        RaycastHit[] hit = Physics.RaycastAll(ray);

        // Get first body part
        foreach (RaycastHit obj in hit)
        {
            if (obj.collider.tag == "BodyPart")
            {
                // found first body part, attach and return
                Attach(obj.collider.GetComponent<BodyPart>());
                Debug.Log("YAAS");
                return true;
            }
        }

        return false;
    }
}
