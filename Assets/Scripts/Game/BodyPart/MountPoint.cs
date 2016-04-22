using UnityEngine;
using System.Collections;

public class MountPoint : MonoBehaviour {

    // Fields
    private RegularItem mountedItem;

    // current item
    public RegularItem MountedItem
    {
        get
        {
            return mountedItem;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Unmonut the item from this mount point and return it.
    /// </summary>
    /// <returns>The unmounted item.</returns>
    public RegularItem UnmountItem()
    {
        RegularItem tmpItem = mountedItem;
        mountedItem = null;
        return tmpItem;
    }

    /// <summary>
    /// Mounts an item to this mount point
    /// </summary>
    /// <param name="item">Item to mount</param>
    /// <returns>True if the item was able to be mounted successfully.</returns>
    public bool MountItem(RegularItem item)
    {
        if(mountedItem == null)
        {
            // set the mounted item
            mountedItem = item;

            // parent it and reset transform
            item.transform.parent = transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            // return true and we're done
            item.PrepareToUse();
            return true;
        }

        return false;
    }

    public void UseItem()
    {
        if (mountedItem != null)
        {
            mountedItem.Use();
            UnmountItem();
        }
    }
}
