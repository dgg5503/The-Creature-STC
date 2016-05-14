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
    private Character characterRef;
    private Player playerRef;
    // public GameObject dropItem;
    private GameObject playerPosition;

    //Health Bars for the creature body parts
   // private GameObject headHealthBar;
    private GameObject leftHandHealthBar;
    private GameObject rightHandHealthBar;
    private GameObject leftLegHealthBar;
    private GameObject rightLegHealthBar;


    // Use this for initialization
    public List<GameObject> AllSlots { get { return allSlots; } }

    protected virtual void Awake()
    {
        if (characterRef == null)
        {
            characterRef = GetComponent<Character>();
        }

        if (playerRef == null)
        {
            playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        //Health Bar
       // headHealthBar = GameObject.FindGameObjectWithTag("HHB");
        leftHandHealthBar = GameObject.FindGameObjectWithTag("LHHB");
        rightHandHealthBar = GameObject.FindGameObjectWithTag("RHHB");
        leftLegHealthBar = GameObject.FindGameObjectWithTag("LLHB");
        rightLegHealthBar = GameObject.FindGameObjectWithTag("RLHB");
        //Debug.Log("HealthBar is here:" + headHealthBar);
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
        //headHealthBar.SetActive(true);
        //headHealthBar.SetActive(false);
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
                        string pathToTheFile = "Prefabs/Items/" + item.name;
                        GameObject test = Resources.Load(pathToTheFile) as GameObject;
                       GameObject itemToInstantiate =  (GameObject)GameObject.Instantiate(test, playerPosition.transform.position - position, Quaternion.identity);
                       itemToInstantiate.name = item.name;
       
                       playerRef.MountItem(itemToInstantiate.GetComponent<RegularItem>());
                    }
                    else
                    {
                        DetachBodyParts(moveFromSlot.name);
                        toggleHealthBars();
                        break;
                    }

                }
                if (moveFromSlot.name != "Head")
                {
                   moveFromSlot.clearItemSlot();
                }
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
            Debug.Log("AddItem From the inventory Class:" + item.name);
            findEmptyAndAdd(item);
            return true;
        }
        return false;
    }

    public void move(GameObject slotClicked)
    {
        if (moveFromSlot == null) // This is the first slot that we clicked
        {
            if (slotClicked.GetComponent<Slot>().IsEmptySlot() != true) // if slot is not empty 
            {
                moveFromSlot = slotClicked.GetComponent<Slot>();

            }
        }
        else if (moveToSlot == null)
        {
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


    public virtual void DetachBodyParts(string Name)
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


    public void reduceHealthImproved(int id, float value)
    {
        float convertedValue = helperFunctionBodyPartHealth(id, value);

        switch (id)
        {
            case 0: //Head
                {
                   // headHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 1: // Left Arm Part 1
                {
                    leftHandHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 2: // Left Arm Part 2
                {
                    leftHandHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 3: // Left Leg part 1
                {
                    leftLegHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 4: // Left Leg part 2
                {
                    leftLegHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 5: // Right Arm part 1
                {
                    rightHandHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 6: // Right Arm part 2
                {
                    rightHandHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 7: // Right Arm part 2
                {
                    rightLegHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
            case 8: // Right Leg
                {
                    rightLegHealthBar.GetComponent<Image>().fillAmount = convertedValue;
                    break;
                }
        }
    }

    /*
    public float helperFunctionBodyPartHealth(int id, float healthValue) {
        float convertedValue = 0.0f;
        for (int i = 0; i < playerRef.BodyParts.Length; i++)
        {
            if (id == 1)
            {
                if (playerRef.BodyParts[i].BodyPartType == 2)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 2)
            {
                if (playerRef.BodyParts[i].BodyPartType == 1)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 3)
            {
                if (playerRef.BodyParts[i].BodyPartType == 4)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 4)
            {
                if (playerRef.BodyParts[i].BodyPartType == 3)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 5)
            {
                if (playerRef.BodyParts[i].BodyPartType == 6)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 6)
            {
                if (playerRef.BodyParts[i].BodyPartType == 5)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 7)
            {
                if (playerRef.BodyParts[i].BodyPartType == 8)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 8)
            {
                if (playerRef.BodyParts[i].BodyPartType == 7)
                {
                    convertedValue = (healthValue + playerRef.BodyParts[i].Health) / 2;
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
        }

        Debug.Log("Converted Value Should be here: " + convertedValue);
        return convertedValue;
    }
    */

    public float helperFunctionBodyPartHealth(int id, float healthValue)
    {
        float convertedValue = 0.0f;
        for (int i = 0; i < playerRef.BodyParts.Length; i++)
        {
            if (id == 1)
            {
                if (playerRef.BodyParts[i].BodyPartType == 2)
                {

                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 2)
            {
                if (playerRef.BodyParts[i].BodyPartType == 1)
                {
                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 3)
            {
                if (playerRef.BodyParts[i].BodyPartType == 4)
                {
                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 4)
            {
                if (playerRef.BodyParts[i].BodyPartType == 3)
                {
                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 5)
            {
                if (playerRef.BodyParts[i].BodyPartType == 6)
                {
                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 6)
            {
                if (playerRef.BodyParts[i].BodyPartType == 5)
                {
                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 7)
            {
                if (playerRef.BodyParts[i].BodyPartType == 8)
                {
                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
            else if (id == 8)
            {
                if (playerRef.BodyParts[i].BodyPartType == 7)
                {
                    convertedValue = Mathf.Min(healthValue, playerRef.BodyParts[i].Health);
                    convertedValue /= 100;
                    break;
                }
                else
                {
                    convertedValue = healthValue / 100;
                }
            }
        }

        //Debug.Log("Converted Value Should be here: " + convertedValue);
        return convertedValue;
    }



    public void toggleHealthBars() {
        bool leftHandHealthBarCondition = (playerRef.IsJointOccupied(1) || playerRef.IsJointOccupied(2)) ? true : false;
        leftHandHealthBar.SetActive(leftHandHealthBarCondition);
        bool rightHandHealthBarCondition = (playerRef.IsJointOccupied(5) || playerRef.IsJointOccupied(6)) ? true : false;
        rightHandHealthBar.SetActive(rightHandHealthBarCondition);
        bool leftLegHealthBarCondition = (playerRef.IsJointOccupied(3) || playerRef.IsJointOccupied(4)) ? true : false;
        leftLegHealthBar.SetActive(leftLegHealthBarCondition);
        bool rightLegHealthBarCondition = (playerRef.IsJointOccupied(7) || playerRef.IsJointOccupied(8)) ? true : false;
        rightLegHealthBar.SetActive(rightLegHealthBarCondition);
    }



    public void toggleBodyPartsIcons() { 
        // Have to call Slotname .clearItemSlot
        bool leftHand = (playerRef.IsJointOccupied(1) || playerRef.IsJointOccupied(2)) ? true : false;
        bool rightHand= (playerRef.IsJointOccupied(5) || playerRef.IsJointOccupied(6)) ? true : false;
        bool leftLeg = (playerRef.IsJointOccupied(3) || playerRef.IsJointOccupied(4)) ? true : false;
        bool rightLeg = (playerRef.IsJointOccupied(7) || playerRef.IsJointOccupied(8)) ? true : false;
        foreach (GameObject slot in allSlots)
        {
            if (slot.name == "LeftHand" && leftHand == false)
            {
                slot.GetComponent<Slot>().clearItemSlot();
            }
            if (slot.name == "RightHand" && rightHand == false)
            {
                slot.GetComponent<Slot>().clearItemSlot();
            }
            if (slot.name == "LeftLeg" && leftLeg == false)
            {
                slot.GetComponent<Slot>().clearItemSlot();
            }
            if (slot.name == "RightLeg" && rightLeg == false)
            {
                slot.GetComponent<Slot>().clearItemSlot();
            }
        }
    }

    public int getNumberOfEmptyRegularItemSlots(){
        int numberOfEmptySlots = 0;
        foreach (GameObject slot in allSlots)
        {
            if (slot.name == "InventorySlot")
            {
                if (slot.GetComponent<Slot>().IsEmptySlot())
                {
                    ++numberOfEmptySlots;
                }
            }
        }
        return numberOfEmptySlots-1;
    }

    //Helper
    public void returnTotalNumberOfSlots() {
        foreach (GameObject slot in allSlots)
        {
            Debug.Log(slot.name);
        }
    }
/*
    public void reduceHealth(BodyPart part, float value)
    {
        float convertedValue = value / 100;

        if (part.name.Contains("Head"))
        {
            headHealthBar.GetComponent<Image>().fillAmount = convertedValue;
        }
        else if (part.name.Contains("Left_Arm"))
        {
            leftHandHealthBar.GetComponent<Image>().fillAmount = convertedValue;
        }
        else if (part.name.Contains("Right_Arm"))
        {
            rightHandHealthBar.GetComponent<Image>().fillAmount = convertedValue;
        }
        else if (part.name.Contains("Left_Leg"))
        {
            leftLegHealthBar.GetComponent<Image>().fillAmount = convertedValue;
        }
        else if (part.name.Contains("Right_Leg"))
        {
            rightLegHealthBar.GetComponent<Image>().fillAmount = convertedValue;
        }
    }
*/
}
