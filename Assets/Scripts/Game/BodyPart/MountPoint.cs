using UnityEngine;
using System.Collections;

/// <summary>
/// Container class for mount points on a character.
/// </summary>
public class MountPoint : MonoBehaviour {
    // current item
    /// <summary>
    /// Get the current item mounted to this mount point.
    /// </summary>
    public RegularItem MountedItem
    {
        get
        {
            return GetComponentInChildren<RegularItem>();
        }
    }

    /// <summary>
    /// Gets the body part type bound to this mount point.
    /// </summary>
    public int BodyPartType { get; private set; }

    public void Awake()
    {
        // set body part type
        BodyPartType = GetComponentInParent<BodyPart>().BodyPartType;
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
