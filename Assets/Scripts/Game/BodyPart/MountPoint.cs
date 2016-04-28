using UnityEngine;
using System.Collections;

/// <summary>
/// Container class for mount points on a character.
/// </summary>
public class MountPoint : MonoBehaviour {
    // current item
    public RegularItem MountedItem
    {
        get
        {
            return GetComponentInChildren<RegularItem>();
        }
    }

    /// <summary>
    /// Use the item contained in this mount point.
    /// </summary>
    public void UseItem()
    {
        if (MountedItem != null)
            MountedItem.Use();
    }
}
