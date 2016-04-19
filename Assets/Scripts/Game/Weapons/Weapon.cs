using UnityEngine;
using System.Collections;

/// <summary>
/// Must have a collider in order for damage to be applied to characters.
/// </summary>
[RequireComponent(typeof(Collider))]

public abstract class Weapon : RegularItem {

    // Fields
    //private new Collider collider;
    //private bool isActive;
    //private BodyPart collidedBodyPart;
    protected int damage;

    // Properties
    /// <summary>
    /// Get whether or not this weapon is ready to apply damage.
    /// </summary>
    /*
    public bool IsActive
    {
        get
        {
            return isActive;
        }
    }
    */

    /// <summary>
    ///  Get the body part that this weapon collided with.
    /// </summary>
    /*
    protected BodyPart CollidedBodyPart
    {
        get
        {
            return collidedBodyPart;
        }
    }
    */
    /*
        - Set item type to weapon.
        - Amount of items is the number of items to stack.
    */
    protected override void Awake()
    {
        base.Awake();
        
        //isActive = true;
        //collider = GetComponent<Collider>();
        type = RegularItemType.Weapon;
        amountOfItems = 1;
        damage = 0;
    }

	// Use this for initialization
	void Start () {
        // warning for 0 damage weapon
        if (damage == 0)
            Debug.LogError("ERROR: " + name + " damage is set to 0.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /*
    void OnCollisionEnter(Collision collision)
    {
        // ensure what we hit was a connected body part
        if (isActive &&
            (collidedBodyPart = collision.collider.GetComponent<BodyPart>()) != null &&
             collidedBodyPart.Joint != null)
        {
            // apply damage
            collidedBodyPart.Health -= damage;

            // apply after functions
            AfterCollision();

            isActive = false;
        }
    }
    */

    /// <summary>
    /// Implement this function in extended file to run instructions after damage has been
    /// successfully applied.
    /// </summary>
    //protected abstract void AfterCollision();
}
