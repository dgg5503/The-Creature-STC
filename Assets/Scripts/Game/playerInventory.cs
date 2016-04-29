using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class playerInventory : Inventory {

    private Player playerRef;
    List<Item> items = new List<Item>();
    List<GameObject> allSlots = new List<GameObject>();
    static int emptySlot; // Check how many empty slots are there;
    private static Slot moveFromSlot; // reference the slot from which we are moving 
    private static Slot moveToSlot; // reference to the slot to which we are moving
    private Character characterRef;
    // public GameObject dropItem;
    private GameObject playerPosition;



	// Use this for initialization
	void Start () {
        playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        foreach (Transform child in transform)
        {
            GameObject slot = child.gameObject;
            allSlots.Add(slot);
        }

        emptySlot = allSlots.Count;
        playerPosition = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (moveFromSlot != null)
            {
                foreach (Item item in moveFromSlot.Items)
                {
                    if (item.GetComponent<RegularItem>())
                    {
                        Vector3 position = new Vector3(1, 0, 1);
                        string pathToTheFile = "Prefabs/Items/" + item.itemName;
                        GameObject test = Resources.Load(pathToTheFile) as GameObject;
                        GameObject.Instantiate(test, playerPosition.transform.position - position, Quaternion.identity);
                    }
                    else
                    {
                        DetachBodyParts(moveFromSlot.name);
                    }

                }

                moveFromSlot.clearItemSlot();
                moveFromSlot = null;
                moveToSlot = null;
                emptySlot++;
            }
        }
	}

    public override void DetachBodyParts(string Name)
    {
        switch (Name)
        {
            case "Head":
                {
                    playerRef.Detach(0);
                    break;
                }
            case "LeftHand":
                {
                    playerRef.Detach(1);
                    break;
                }
            case "RightHand":
                {
                    playerRef.Detach(5);
                    break;
                }
            case "LeftLeg":
                {
                    playerRef.Detach(3);
                    break;
                }
            case "RightLeg":
                {
                    playerRef.Detach(7);
                    break;
                }
        }
    }
}
