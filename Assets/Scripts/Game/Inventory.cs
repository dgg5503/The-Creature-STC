using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Inventory : MonoBehaviour
{

    List<Item> items = new List<Item>();
    List<GameObject> allSlots = new List<GameObject>();
    static int emptySlot; // Check how many empty slots are there;
    private static Slot moveFromSlot; // reference the slot from which we are moving 
    private static Slot moveToSlot; // reference to the slot to which we are moving
    static Player playerRef;
    private BodyPart bodyRef;
    // public GameObject dropItem;
    private GameObject playerPosition;



    // Use this for initialization


    protected virtual void Awake()
    {
        if (playerRef == null)
        {
            playerRef = GetComponent<Player>();
        }
    }

    void Start()
    {
       
        foreach (Transform child in transform)
        {
            GameObject slot = child.gameObject;
            allSlots.Add(slot);
        }
        
        emptySlot = allSlots.Count;
        playerPosition = GameObject.FindGameObjectWithTag("Player");
       
    }

    // Update is called once per frame
    void Update()
    {
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
                        switch (moveFromSlot.name)
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

                moveFromSlot.clearItemSlot();
                moveFromSlot = null;
                moveToSlot = null;
                emptySlot++;
            }
        }
    }

    /// <summary>
    /// Search for an empty slot and add to it
    /// </summary>
    /// <param name="item">Item that should be added</param>
    /// <returns></returns>
    private bool findEmptyAndAdd(Item item)
    {
        Debug.Log(emptySlot);
        Debug.Log("Adding Item: " + item);
        if (emptySlot > 0)
        {
            foreach (GameObject slot in allSlots)
            {

                Slot tempSlot = slot.GetComponent<Slot>();

                switch (tempSlot.name)
                {
                    case "InventorySlot":
                        {
                            if (item.GetComponent<RegularItem>())
                            {
                                if (tempSlot.IsEmptySlot())
                                {
                                    tempSlot.AddItem(item);
                                    emptySlot--;
                                    return true;
                                } // end of if
                            }
                            break;
                        }
                    case "Head":
                        {
                            if (item.GetComponent<BodyPart>() && item.GetComponent<BodyPart>().name.Contains("Head"))
                            {
                                if (tempSlot.IsEmptySlot())
                                {
                                    tempSlot.AddItem(item);
                                    emptySlot--;
                                    return true;
                                    Debug.Log("Item ADDDED HERE");
                                } // end of if
                            }
                            break;
                        }
                    case "LeftHand":
                        {
                            if (item.GetComponent<BodyPart>() && item.GetComponent<BodyPart>().name.Contains("Left_Arm"))
                            {
                                if (tempSlot.IsEmptySlot())
                                {
                                    tempSlot.AddItem(item);
                                    emptySlot--;
                                    return true;
                                } // end of if
                            }
                            break;
                        }
                    case "RightHand":
                        {
                            if (item.GetComponent<BodyPart>() && item.GetComponent<BodyPart>().name.Contains("Right_Arm"))
                            {
                                if (tempSlot.IsEmptySlot())
                                {
                                    tempSlot.AddItem(item);
                                    emptySlot--;
                                    return true;
                                } // end of if
                            }
                            break;
                        }
                    case "LeftLeg":
                        {
                            if (item.GetComponent<BodyPart>() && item.GetComponent<BodyPart>().name.Contains("Left_Leg"))
                            {
                                if (tempSlot.IsEmptySlot())
                                {
                                    tempSlot.AddItem(item);
                                    emptySlot--;
                                    return true;
                                } // end of if
                            }
                            break;
                        }
                    case "RightLeg":
                        {
                            if (item.GetComponent<BodyPart>() && item.GetComponent<BodyPart>().name.Contains("Right_Leg"))
                            {
                                if (tempSlot.IsEmptySlot())
                                {
                                    tempSlot.AddItem(item);
                                    emptySlot--;
                                    return true;
                                } // end of if
                            }
                            break;
                        }
                }
            } //end of foreach
        } // end of if

        return false;
    }

    /// <summary>
    /// Add item to an empty slot 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool AddItem(Item item)
    {
        if (item.GetComponent<RegularItem>())
        {
            if (item.GetComponent<RegularItem>().amountOfItems == 1)
            {
                findEmptyAndAdd(item);
                return true;
            }
            else
            {
                foreach (GameObject slot in allSlots)
                {
                    Slot tempSlot = slot.GetComponent<Slot>();
                    //check if slot is not empty
                    if (tempSlot.IsEmptySlot() != true)
                    {
                        // compare the items and check if you can stack more items in that slot.
                        //if you can, then stack items in that slot
                        if (tempSlot.itemInTheSlot() == item && tempSlot.stackAvailable())
                        {
                            tempSlot.AddItem(item);
                            return true;
                        }
                    }
                }
                //if slot is full
                // stack in a new slot.
                if (emptySlot > 0)
                {
                    findEmptyAndAdd(item);
                }
            }
        }
        else if (item.GetComponent<BodyPart>())
        {
            findEmptyAndAdd(item);
            return true;
        }
        return false;
    }

    public void move(GameObject slotClicked)
    {
        Debug.Log("I'm here");
        if (moveFromSlot == null) // This is the first slot that we clicked
        {
            if (slotClicked.GetComponent<Slot>().IsEmptySlot() != true) // if slot is not empty 
            {
                moveFromSlot = slotClicked.GetComponent<Slot>();

            }
        }
        else if (moveToSlot == null)
        {
            Debug.Log("Second SLOT");
            moveToSlot = slotClicked.GetComponent<Slot>();
        }

        if (moveFromSlot != null && moveToSlot != null)
        {
            if (moveToSlot.name == "InventorySlot" && moveFromSlot.name == "InventorySlot")
            {
                List<Item> tempHold = new List<Item>(moveToSlot.Items);
                moveToSlot.addMultipleItems(moveFromSlot.Items);
                if (tempHold.Count == 0)
                {
                    moveFromSlot.clearItemSlot();
                }
                else
                {
                    moveFromSlot.addMultipleItems(tempHold);
                }

                moveToSlot = null;
                moveFromSlot = null;
            }
            else
            {
                moveToSlot = null;
                moveFromSlot = null;
            }
        }

    }
}
