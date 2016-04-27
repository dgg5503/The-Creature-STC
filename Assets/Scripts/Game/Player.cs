﻿/*
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

    // TMP INVENTORY
    private GameObject characterInventory;
    private bool displayCharacterInventory = true;
    public Inventory charInventory;
    private GameObject headHealthBar;
    private GameObject leftHandHealthBar;
    private GameObject rightHandHealthBar;
    private GameObject leftLegHealthBar;
    private GameObject rightLegHealthBar;
    private int cheatIWay = 1;
    private string path;

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
        headHealthBar = GameObject.FindGameObjectWithTag("HHB");
        leftHandHealthBar = GameObject.FindGameObjectWithTag("LHHB");
        rightHandHealthBar = GameObject.FindGameObjectWithTag("RHHB");
        leftLegHealthBar = GameObject.FindGameObjectWithTag("LLHB");
        rightLegHealthBar = GameObject.FindGameObjectWithTag("RLHB");
        characterInventory.SetActive(false);
     //   getCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
     //   passCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); 
    }

    // Use this for initialization
    void Start()
    {
        characterInventory.SetActive(true);

        CheatWay();
    }

    //int tmpIndex = 0;
    //BodyPart tmpBodyPart = null;
    // Update is called once per frame
    protected override void Update()
    {
        
        if (cheatIWay <= 1)
        {
            CheatWay();
            cheatIWay++;
        }
        

        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Item")
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Debug.Log(hit.transform.gameObject.GetComponent<Item>().itemName);

                    if (hit.transform.gameObject.GetComponent<BodyPart>())
                    {
                        path = "Prefabs/BodyParts/";
                    }
                    else if (hit.transform.gameObject.GetComponent<RegularItem>())
                    {
                        path = "Prefabs/Items/";
                        Destroy(hit.transform.gameObject);
                    }
                    GameObject itemToAdd = Resources.Load(path + hit.transform.gameObject.GetComponent<Item>().name) as GameObject;
                    Debug.Log(itemToAdd);
                    Item itemToAddNew = itemToAdd.GetComponent<Item>() as Item;
                    charInventory.AddItem(itemToAddNew);
                   // Destroy(hit.transform.gameObject);
                }
            }
        }

        /* DEBUG TESTS */
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
            foreach (RaycastHit obj in allhit)
                if ((foundBodyPart = obj.collider.GetComponent<BodyPart>()) != null)
                    break;

            // Detach if parented
            if (foundBodyPart != null)
                if (Detach(foundBodyPart.BodyPartType) == null)
                    Attach(foundBodyPart);
        }

        if(Input.GetKeyDown("k"))
        {
            Spear[] spears = GetComponentsInChildren<Spear>();
            if(spears.Length != 0)
                MountItem(spears[0]);
        }

        if(Input.GetKeyDown("j"))
        {
            MountPoint[] mountPoints = GetComponentsInChildren<MountPoint>();
            for (int i = 0; i < mountPoints.Length; ++i)
                mountPoints[i].UseItem();
        }

        /* DEBUG TESTS */
        // TMP INVENTORY
        if (Input.GetKeyDown("i"))
        {
            for (int i = 0; i < this.BodyParts.Length; i++)
            {
                charInventory.AddItem(this.BodyParts[i]);
            }
            displayCharacterInventory = !displayCharacterInventory;
            Debug.Log("Status of the inventory: " + displayCharacterInventory);
            characterInventory.SetActive(displayCharacterInventory);
            Debug.Log("Check Inventory Again: " + characterInventory.active);
        }

        if (Input.GetKeyDown("m"))
        {
            headHealthBar.GetComponent<Image>().fillAmount -= 0.1f;
            leftHandHealthBar.GetComponent<Image>().fillAmount -= 0.1f;
            rightHandHealthBar.GetComponent<Image>().fillAmount -= 0.1f;
            leftLegHealthBar.GetComponent<Image>().fillAmount -= 0.1f;
            rightLegHealthBar.GetComponent<Image>().fillAmount -= 0.1f;

           // DontDestroyOnLoad(this.transform.gameObject);
           // DontDestroyOnLoad(getCanvas);
          //  DontDestroyOnLoad(passCamera);
           // SceneManager.LoadScene("TestSceneForTransferObjects");
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

        
        #endregion
    }

    // TODO REMOVE THIS FUNCTION AND ADD ACTUAL TREE DATA STRUCTURE
    private GameObject GetRoot(GameObject bodyPart)
    {
        if (bodyPart.transform.parent == null)
            return bodyPart;
        return GetRoot(bodyPart.transform.parent.gameObject);
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
    
}
