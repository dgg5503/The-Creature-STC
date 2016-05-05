/*
    Item Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - 
*/

using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour {

    public Sprite itemIcon; // Icon displayed in the inventory
    public string itemName;


    protected virtual void Awake()
    {
        // set layer of items to layer 9
        gameObject.layer = 9;

        tag = "Item";
    }

    
}
