/*
    Player Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - 
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
public class Player : Character
{
    // Camera used by this player
    [SerializeField]
    private Camera playerCamera = null;
    private Vector3 cameraForward;
    private Vector3 cameraRight;

    // Pickup distance
    [SerializeField]
    private float pickupDistance = 4f;

    // TMP INVENTORY
    private GameObject characterInventory;
    private bool displayCharacterInventory = true;
    public Inventory charInventory;
    private int cheatIWay = 1;
    private string path;
    private BodyPart testingBodyPart;
    private string newItemName = "";

    //TEst
    // private Canvas getCanvas;
    //  private Camera passCamera;
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
        rotationAccelFactor = 6f;
        maxSpeed = 2.0f;

        characterInventory = GameObject.FindGameObjectWithTag("Inventory");
        characterInventory.SetActive(false);

        //testingBodyPart = GameObject.Find("Creature_Left_Leg_Part_1").GetComponent<BodyPart>();
        //   getCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //   passCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // -- KEY BINDING -- //
        GameManager.InputManager.BindKey(MoveForward, KeyCode.W);
        GameManager.InputManager.BindKey(MoveLeft, KeyCode.A);
        GameManager.InputManager.BindKey(MoveBackward, KeyCode.S);
        GameManager.InputManager.BindKey(MoveRight, KeyCode.D);
        GameManager.InputManager.BindKey(UseItemLeft, KeyCode.E);
        GameManager.InputManager.BindKey(UseItemRight, KeyCode.Q);
        GameManager.InputManager.BindKey(ToggleInventory, KeyCode.I);
    }

    // Use this for initialization
    protected override void Start()
    {
        characterInventory.SetActive(true);

        CheatWay();
        charInventory.toggleHealthBars();

        base.Start();
    }

    bool attachSuccess;
    protected override void Update()
    {
        // TMP FIX: REMOVE FOR FINAL
        if (cheatIWay <= 1)
        {
            CheatWay();
            cheatIWay++;
        }
        

        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Distance(hit.point, transform.position) < pickupDistance &&
                hit.transform.tag == "Item")
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (hit.transform.gameObject.GetComponent<BodyPart>())
                    {
                        //BodyPart bodyPart = (hit.transform.gameObject.GetComponent<BodyPart>()).GetRootBodyPart();
                        BodyPart bodyPart = hit.transform.gameObject.GetComponent<BodyPart>();
                        // make sure we're at parent
                        path = "Prefabs/BodyParts/";
                        if (joints[bodyPart.BodyPartType].BodyPart != null)
                            return;

                        if (bodyPart.IsControlledByJoint == false)
                            Detach(bodyPart.BodyPartType);

                        if (attachSuccess = Attach(bodyPart))
                        {
                            charInventory.toggleBodyPartsIcons();
                            charInventory.reduceHealthImproved(bodyPart.BodyPartType, bodyPart.Health);
                            charInventory.toggleHealthBars();
                        }
                        CheatWay();
                    }
                    else if (hit.transform.gameObject.GetComponent<RegularItem>())
                    {
                        path = "Prefabs/Items/";
                        if (charInventory.getNumberOfEmptyRegularItemSlots() >= 0)
                        {
                            if (hit.transform.gameObject.GetComponent<RegularItem>().CurrentMountPoint == null)
                            Destroy(hit.transform.gameObject);
                        }   
                    }
                    string pickUpItemName = hit.transform.gameObject.name;
                    if (pickUpItemName.Any(char.IsWhiteSpace))
                    {
                        newItemName = pickUpItemName.Remove(pickUpItemName.IndexOf(' '));
                    }
                    else if (pickUpItemName.Contains('('))
                    {
                        newItemName = pickUpItemName.Remove(pickUpItemName.IndexOf('('));
                    }
                    else
                    {
                        newItemName = pickUpItemName;
                    }

                    if (hit.transform.gameObject.GetComponent<RegularItem>())
                    {
                        if (hit.transform.gameObject.GetComponent<RegularItem>().CurrentMountPoint == null)
                        {
                            GameObject itemToAdd = Resources.Load(path + newItemName) as GameObject;
                            Item itemToAddNew = itemToAdd.GetComponent<Item>() as Item;
                            charInventory.AddItem(itemToAddNew);
                        }
                    }
                    else
                    {
                        if (attachSuccess)
                        {
                            GameObject itemToAdd = Resources.Load(path + hit.transform.gameObject.name) as GameObject;
                            Item itemToAddNew = itemToAdd.GetComponent<Item>() as Item;
                            charInventory.AddItem(itemToAddNew);
                        }
                    }
                }
            }
        }

        /* DEBUG TESTS */
        /*
        if (Input.GetMouseButtonDown(0))
        {
            // ray cast
            // RAY CAST
            // cast ray from camera to where mouse position is
            ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            // Raycast
            RaycastHit[] allhit = Physics.RaycastAll(ray);

            // Get first body part
            BodyPart foundBodyPart = null;
            RegularItem foundItem = null;
            foreach (RaycastHit obj in allhit)
            {
                if ((foundBodyPart = obj.collider.GetComponent<BodyPart>()) != null)
                {
                    //if (Detach(foundBodyPart.BodyPartType) == null)
                    //{
                        //Attach(foundBodyPart);
                        charInventory.toggleBodyPartsIcons();
                       // CheatWay();
                    //}
                    charInventory.reduceHealthImproved(foundBodyPart.BodyPartType, foundBodyPart.Health);
                    charInventory.toggleHealthBars();
                    CheatWay();
                    break;
                }

                if ((foundItem = obj.collider.GetComponent<RegularItem>()) != null)
                {
                    // attempt to mount
                    MountItem(foundItem);
                    break;
                }
                    
            }
        }
        */
        /* DEBUG WEAPON TEST */
        if(Input.GetKeyDown("k"))
        {
            Weapon[] spears = GetComponentsInChildren<Weapon>().Where(x => x.CurrentMountPoint == null).ToArray();

            // if weapon is already mounted, dont mount again!
            if(spears.Length != 0)
                MountItem(spears[0]);
        }

        /* DEBUG WEAPON TEST */
        /*
        if (Input.GetKeyDown("j"))
        {
            MountPoint[] mountPoints = GetComponentsInChildren<MountPoint>();
            //state = CharacterState.Throw_Right;
            for (int i = 0; i < mountPoints.Length; ++i)
                if (mountPoints[i].MountedItem != null)
                {
                    mountPoints[i].UseItem();
                    break;
                }
        }
        */

        /* DEBUG TESTS */
        // TMP INVENTORY
        /*
        if (Input.GetKeyDown("i"))
        {
            
        }
        */
        
        /*
        if (Input.GetKeyDown("m"))
        {
            testingBodyPart.Health = 20;
           // DontDestroyOnLoad(this.transform.gameObject);
           // DontDestroyOnLoad(getCanvas);
          //  DontDestroyOnLoad(passCamera);
           // SceneManager.LoadScene("TestSceneForTransferObjects");
        }
        */
        base.Update();
        
    }

    /*
    * 1. Execute individual states based on conditions
    *  a. Idle - not hitting button (REQUIRED)
    *  b. Aim - button held down (optional)
    *  c. Executing - button let go (REQUIRED)
    *  d. Success - item hit target (REQUIRED)
    *  e. Failure - item failed to hit target (optional)
    * 2. Get item state name from current mount point ID
    * 3. Set the item ID in animator
    * 4. Set the item state in animator
    *  
    */
    /*
    public void UseItem(RegularItem item)
    {
        // check to see if item state already set
        switch (item.ItemState)
        {
            case ItemState.Idle: // no keys
                break;

            case ItemState.Aim: // key held down
                break;

            case ItemState.Executing: // key let go
                break;

            case ItemState.Success: // item finished its action successfully
                break;

            case ItemState.Failure: // item failed to finish its action.
                break;
        }

        characterAnimator.SetInteger(item.ItemStateTag, (int)item.ItemState);
    }
    */

    /// <summary>
    /// Processes movement for the player specifically. Called before applying
    /// changes in acceleration.
    /// </summary>
    protected override void ProcessMovement()
    {
        // Get camera forward and right
        cameraForward = playerCamera.transform.forward;
        cameraRight = playerCamera.transform.right;

        #region movement
        /*
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
        */
        // apply accel scalar AFTER getting direction
        // this is so we dont add two times the accel scalar 
        // when holding down two direction at the same time.
        acceleration *= accelerationScalar;
        
        #endregion
    }

    private void CheatWay()
    {
        for (int i = 0; i < this.BodyParts.Length; i++)
        {
            charInventory.AddItem(this.BodyParts[i]);
        }
     //   displayCharacterInventory = !displayCharacterInventory;
      //  Debug.Log("Status of the inventory: " + displayCharacterInventory);
      //  characterInventory.SetActive(displayCharacterInventory);
      //  Debug.Log("Check Inventory Again: " + characterInventory.active);
    }

    // --- INPUT FUNCTIONS ---
    private void MoveForward(KeyState keyState)
    {
        acceleration.x += cameraForward.x;
        acceleration.z += cameraForward.z;
    }

    private void MoveBackward(KeyState keyState)
    {
        acceleration.x += cameraForward.x * -1;
        acceleration.z += cameraForward.z * -1;
    }

    private void MoveLeft(KeyState keyState)
    {
        acceleration.x += cameraRight.x * -1;
        acceleration.z += cameraRight.z * -1;
    }

    private void MoveRight(KeyState keyState)
    {
        acceleration.x += cameraRight.x;
        acceleration.z += cameraRight.z;
    }

    private void UseItemLeft(KeyState keyState)
    {
        AimingAt = transform.forward;
        UseItem(CreatureBodyBones.Left_Arm_Part_2, keyState);
    }

    private void UseItemRight(KeyState keyState)
    {
        AimingAt = transform.forward;
        UseItem(CreatureBodyBones.Right_Arm_Part_2, keyState);
    }

    private void ToggleInventory(KeyState keyState)
    {
        if(keyState == KeyState.KEY_DOWN)
        {
            for (int i = 0; i < this.BodyParts.Length; i++)
            {
                charInventory.AddItem(this.BodyParts[i]);
            }
            displayCharacterInventory = !displayCharacterInventory;
            characterInventory.SetActive(displayCharacterInventory);
        }
    }

    private void UseSpecialItem(KeyState keyState)
    {
        if (keyState == KeyState.KEY_DOWN)
        {
            // holding down!!
        }
        else if (keyState == KeyState.KEY_UP)
        {
            // let go!!
        }
    }

}
