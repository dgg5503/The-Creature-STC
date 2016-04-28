using UnityEngine;
using System.Collections;

/// <summary>
/// Must have a collider in order for damage to be applied to characters.
/// </summary>
[RequireComponent(typeof(Collider))]

/// <summary>
/// Colliders require rigidbodies.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class Weapon : RegularItem {

    // Fields
    protected int damage;
    protected new Rigidbody rigidbody;
    protected new Collider collider;

    /*
        - Set item type to weapon.
        - Amount of items is the number of items to stack.
    */
    protected override void Awake()
    {
        base.Awake();

        //isActive = true;
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
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
}
