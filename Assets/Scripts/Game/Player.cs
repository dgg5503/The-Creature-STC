/*
    Player Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - 
*/

using UnityEngine;
using System.Collections;

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
        accelerationScalar = 5.0f;
    }

    // Use this for initialization
    void Start () {
	    
	}

    // Update is called once per frame
    int i;
    protected override void Update () {
        // zero out accel
        acceleration = Vector3.zero;

        // Get camera forward
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        #region movement
        if (Input.GetKeyDown(KeyCode.W))
        {
            acceleration += cameraForward * accelerationScalar;
        }

        if(Input.GetKeyUp(KeyCode.W))
        {
            acceleration -= cameraForward * accelerationScalar;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            acceleration += cameraRight * accelerationScalar * -1;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            acceleration -= cameraRight * accelerationScalar * -1;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            acceleration += cameraForward * accelerationScalar * -1;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            acceleration -= cameraForward * accelerationScalar * -1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            acceleration += cameraRight * accelerationScalar;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            acceleration -= cameraRight * accelerationScalar;
        }
        #endregion

        if (Input.GetMouseButtonDown(0))
        {
            if (!CheckForAndAttachBodyPart())
                for (i = 0; i < bodyParts.Count && Detach(i) == null; i++) ;
        }

        base.Update();
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
