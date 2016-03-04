/*
    Player Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - 
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : Character {
    // Camera used by this player
    private Camera playerCamera;

    // TMP INVENTORY
    private GameObject characterInventory;
    private bool displayCharacterInventory = false;

    // TMP VAR
    private float minRaidus;
    private BodyPart holdingBPart;
    private BodyPart handInUse;


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
        rotationAccelFactor = 5f;
        maxSpeed = 5.0f;
        minRaidus = 4.0f;

        characterInventory = GameObject.FindGameObjectWithTag("Inventory");
        characterInventory.SetActive(false);

        holdingBPart = null;
        handInUse = null;
    }

    // Use this for initialization
    void Start () {
	    
	}

    // Update is called once per frame
    int i;
    protected override void Update () {

        // pick up items
        if(Input.GetKeyDown(KeyCode.E))
        {
            // TODO: ALERT, CAN DROP BPARTS EVEN AFTER DETACHING HAND REQUIRED!!!!!!!!!!!!!!!!
            // if already holding bpart...
            if(holdingBPart || handInUse)
            {
                // drop it
                Physics.IgnoreCollision(handInUse.GetComponent<Collider>(), holdingBPart.GetComponent<Collider>(), false);

                Destroy(holdingBPart.gameObject.GetComponent<HingeJoint>());

                holdingBPart.transform.parent = null;
                // TODO FIX SPAZZING WHEN ATTACHING RB TO OBJECT
                holdingBPart.GetComponent<Rigidbody>().velocity = Vector3.zero;
                holdingBPart = null;
                handInUse = null;
                return;
            }

            // cast sphere 
            // TODO: CREATE RANGE OF SOME SORT WITH ANGLE (DOT PROD???)
            Ray sphereCastRay = new Ray(transform.position, Vector3.forward);
            RaycastHit[] objectsHit = Physics.SphereCastAll(sphereCastRay, minRaidus);

            // get closest bpart
            BodyPart closestBodyPart = null;
            float smallestDist = minRaidus + 1;
            // foreach thru each object
            foreach (RaycastHit hit in objectsHit)
            {
                if(hit.collider.gameObject.tag == "BodyPart")
                {
                    BodyPart tmpPart = hit.collider.GetComponent<BodyPart>();
                    float tmpDist = Vector3.Distance(tmpPart.transform.position, transform.position);
                    if (tmpDist < smallestDist && tmpPart.transform.parent == null)
                    {
                        smallestDist = tmpDist;
                        closestBodyPart = tmpPart;
                    }
                }
            }

            // TODO: ACTUALLY MAKE THIS ATTACH TO BODY PART ITSELF!!!
            // pick it up via attaching to hand (ignore collision with hand)
            if (closestBodyPart != null)
            {
                // TODO OPTIMIZE HORRIBLE CHECK
                // if no available hands, return
                KeyValuePair<string, BodyPart> bodyPartTmp = bodyParts.FirstOrDefault(bp => bp.Key.Contains("hand"));
                if (bodyPartTmp.Equals(new KeyValuePair<string, BodyPart>()))
                    return;

                BodyPart bodyPartToAttach = bodyPartTmp.Value;
                if (bodyPartToAttach != null)
                {
                    // ignore collision between available hand items
                    Physics.IgnoreCollision(bodyPartToAttach.GetComponent<Collider>(), closestBodyPart.GetComponent<Collider>());

                    // set parent to available hand
                    closestBodyPart.transform.parent = bodyPartToAttach.transform;

                    // set local position to 00
                    //closestBodyPart.transform.localPosition = bodyPartToAttach.transform.localPosition;
                    closestBodyPart.transform.localPosition = closestBodyPart.transform.TransformDirection(Vector3.down * 5);

                    // vector3 down relative to closestBodyPart

                    // create hinge
                    HingeJoint hinge = closestBodyPart.gameObject.AddComponent<HingeJoint>();
                    hinge.connectedBody = bodyPartToAttach.GetComponent<Rigidbody>();
                    hinge.anchor = closestBodyPart.transform.position + closestBodyPart.transform.TransformDirection(Vector3.down * 5);

                    // set currently holding bpart
                    holdingBPart = closestBodyPart;

                    // set hand in use 
                    handInUse = bodyPartToAttach;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // RAY CAST
            // cast ray from camera to where mouse position is
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            // Raycast
            RaycastHit[] hit = Physics.RaycastAll(ray);

            // Get first body part
            foreach (RaycastHit obj in hit)
            {
                if (obj.collider.tag == "BodyPart")
                {
                    // make sure the PLAYER 
                    // TODO: GET RID OF BAD DETECTION
                    if (Detach(obj.collider.GetComponent<BodyPart>()) == null)
                        Attach(obj.collider.GetComponent<BodyPart>()); // else attach it!

                    // MAKE SURE NEAR BY
                    Character otherGuyRoot = GetRoot(obj.collider.gameObject).GetComponent<Character>();
                    if (otherGuyRoot != null && Vector3.Distance(transform.position, obj.transform.position) <= minRaidus)
                    {
                        
                        if (otherGuyRoot.name != "Player")
                        {
                            // DETACH / ATTACH
                            Attach(otherGuyRoot.Detach(obj.collider.gameObject.GetComponent<BodyPart>()));
                        }
                        // DETACH AND ATTACH
                    }
                    /*
                    if (obj.collider.tag == "Corpse")
                    {
                        // make sure NEAR BY
                        if (Vector3.Distance(transform.position, obj.transform.position) <= minRaidus)
                        {
                            // DETACH 
                        }
                    }
                    */
                }
            }


            // DETACH
            /*
            if (!CheckForAndAttachBodyPart())
                foreach (string s in bodyParts.Keys)
                    if (Detach(s) != null)
                        break;
                        */
        }

        // TMP INVENTORY
        if (Input.GetKeyDown("i"))
        {
            displayCharacterInventory = !displayCharacterInventory;
            characterInventory.SetActive(displayCharacterInventory);
        }

        base.Update();
    }

    /// <summary>
    /// Processes movement for the player specifically. Called before applying
    /// changes in acceleration.
    /// </summary>
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

    /// <summary>
    /// Function that handles clicking on and reattaching body parts in the scene.
    /// </summary>
    /// <returns>True if clicked on bodypart was reattached, false if not.</returns>
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
                return true;
            }
        }

        return false;
    }


    // TODO REMOVE THIS FUNCTION AND ADD ACTUAL TREE DATA STRUCTURE
    private GameObject GetRoot(GameObject bodyPart)
    {
        if (bodyPart.transform.parent == null)
            return bodyPart;
        return GetRoot(bodyPart.transform.parent.gameObject);
    }

}
