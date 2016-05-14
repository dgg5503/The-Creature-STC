using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class Slot : MonoBehaviour
{

    private List<Item> items; // items that we can stack
    public Sprite standart;
    public List<Item> Items
    {
        get { return items; }
        set { items = value; }
    }

    public Text numberOfItems; // display the number of items 
    // Use this for initialization
    void Start()
    {
        items = new List<Item>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Add Items to the slot
    /// </summary>
    /// <param name="item">Items that is added to the slot</param>
    public void AddItem(Item item)
    {

        items.Add(item);
        if (items.Count > 1)
        {
            numberOfItems.text = items.Count.ToString();
        }

        Debug.Log("AddItem from the Slot Class " + item.itemIcon.name);
        SwapIcon(item.itemIcon);
    }

    //Function that handle multiple stack items
    public void addMultipleItems(List<Item> itemsAdd)
    {
        items = new List<Item>(itemsAdd); // copy the stacked items 
        SwapIcon(itemInTheSlot().itemIcon); // swap the icon

        // check how many items are there. It its more then one
        // change the text label to appropriate number
        // else leave the text field blank
        if (items.Count > 1)
        {
            numberOfItems.text = items.Count.ToString();
        }
        else
        {
            numberOfItems.text = "";
        }

    }

    // Check if slot is empty or not
    public bool IsEmptySlot()
    {
        if (items.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Handle Swaping icon's inside the inventory
    private void SwapIcon(Sprite itemIconSprite)
    {

        GetComponent<Image>().sprite = standart;
        GetComponent<Button>().image.sprite = itemIconSprite;

    }

    public void clearItemSlot()
    {
        items.Clear();
        GetComponent<Image>().sprite = standart;
        if (this.name == "InventorySlot")
        {
            numberOfItems.text = null;
        }
        // GetComponent<Button>().image.sprite = standart;
        //GetComponent<Button>().image.sprite = Resources.Load<Sprite>("initSlot");
    }

    // Get the top item in the slot
    public Item itemInTheSlot()
    {
        return items[items.Count - 1];
    }

    //Check if you can stack multipleItems in the slot
    //Depends on an item
    public bool stackAvailable()
    {
        if (itemInTheSlot().GetComponent<RegularItem>())
        {
            if (itemInTheSlot().GetComponent<RegularItem>().amountOfItems > items.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }
}
