using UnityEngine;
using System.Collections;


public enum RegularItemType
{
    Weapon,
    Quest
};

public abstract class RegularItem : Item  {

    public RegularItemType type; // Type of the item
    public int amountOfItems; // How many items in one slot
    public void itemSettings(Sprite itemIcon, RegularItemType typeOfTheItem, int amountOfItems)
    {
        base.itemIcon = itemIcon;
        type = typeOfTheItem;
        this.amountOfItems = amountOfItems;

    }

    public abstract void PrepareToUse();

    public abstract void Use();
    
}
