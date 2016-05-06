using UnityEngine;
using System.Collections.Generic;


public enum RegularItemType
{
    Weapon,
    Quest
};

public struct ItemAnimationInfo
{
    public string itemState;
    public string aimState;
    public string throwState;
    public string equipTrigger;
    public string layerName;
    public string unequipTrigger;

    public ItemAnimationInfo(string itemState,
        string aimState,
        string throwState,
        string equipTrigger,
        string layerName,
        string unequipTrigger)
    {
        this.itemState = itemState;
        this.aimState = aimState;
        this.throwState = throwState;
        this.equipTrigger = equipTrigger;
        this.layerName = layerName;
        this.unequipTrigger = unequipTrigger;
    }
}

public abstract class RegularItem : Item
{
    // dictionary <bodypartTypes, layerIndex>

    /// <summary>
    /// Get the current mount point for this item.
    /// </summary>
    public MountPoint CurrentMountPoint { get; private set; }

    /// <summary>
    /// Get whether or not this item can be used while not mounted.
    /// </summary>
    [SerializeField]
    public bool IsUsableWhileNotMounted { get; private set; }

    /// <summary>
    /// Get or set the item state tags used with character animators.
    /// </summary>
    public Dictionary<int, ItemAnimationInfo> ItemAnimation { get; protected set; }

    /// <summary>
    /// Get or set the point of the fire animation in which this item should actually be used.
    /// </summary>
    public float UseAnimationOffset { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        ItemAnimation = new Dictionary<int, ItemAnimationInfo>();

        // see if we're already a child of a mountpoint!
        // if so, remount, ignores collision!
        MountPoint tmpMP;
        if ((tmpMP = transform.GetComponentInParent<MountPoint>()) != null)
            MountTo(tmpMP);
    }

    public RegularItemType type; // Type of the item
    public int amountOfItems; // How many items in one slot
    public void itemSettings(Sprite itemIcon, RegularItemType typeOfTheItem, int amountOfItems)
    {
        base.itemIcon = itemIcon;
        type = typeOfTheItem;
        this.amountOfItems = amountOfItems;

    }
    /// <summary>
    /// Mounts this item to a given mount point.
    /// </summary>
    /// <param name="newMount">The mountpoint to mount this item to.</param>
    /// <returns>The regular item just mounted if mount was successful. Otherwise, null.</returns>
    public virtual RegularItem MountTo(MountPoint newMount)
    {
        // null check
        // mount point full check.
        if (CurrentMountPoint != null ||
            newMount == null ||
            newMount.MountedItem != null)
            return null;

        if (!MountCheck())
        {
            Debug.Log(transform.name + " cant attach!");
            return null;
        }

        // see if we're mounted to something, if so, unmount first
        //if (MountPoint != null)
        //    Unmount();

        // parent it and reset transform
        transform.parent = newMount.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // ignore collision if parent and child have colliders
        Collider parentCollider;
        Collider itemCollider;
        if ((parentCollider = newMount.GetComponentInParent<Collider>()) != null &&
            (itemCollider = GetComponent<Collider>()) != null)
        {
            Physics.IgnoreCollision(parentCollider, itemCollider);
            //Debug.Log("coll ignored " + parentCollider.name + " and " + itemCollider.name);
        }

        // now mounted.
        CurrentMountPoint = newMount;

        return this;
    }

    // spear would be use + unmount, same time?
    /// <summary>
    /// Unmounts this item from the current mount point (if any).
    /// </summary>
    /// <returns>The regular item that was unmounted if successfull. Otherwise, null.</returns>
    public virtual RegularItem Unmount()
    {
        // null check
        if (CurrentMountPoint == null)
            return null;

        // reapply collision
        Collider parentCollider;
        Collider itemCollider;
        if ((parentCollider = CurrentMountPoint.GetComponentInParent<Collider>()) != null &&
            (itemCollider = GetComponent<Collider>()) != null)
        {
            // TODO: TMP FIX FOR SPEARS
            //Physics.IgnoreCollision(parentCollider, itemCollider, false);
            //Debug.Log("coll unignored " + parentCollider.name + " and " + itemCollider.name);
        }

        // unparent
        transform.parent = null;

        // now unmounted
        CurrentMountPoint = null;

        return this;
    }

    /// <summary>
    /// Optionally call this function 
    /// </summary>
    /// <returns></returns>
    protected abstract bool MountCheck();

    /// <summary>
    /// Suggested to override this function. State set for animation.
    /// </summary>
    public abstract void Use();
}
