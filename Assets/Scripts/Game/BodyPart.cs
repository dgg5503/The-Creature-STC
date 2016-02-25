/*
    Body Part Clas
    ------------------------
    TO-DO
        - Detach()
        - Attach(Character c)

    NOTES
        - Attach this script to the root of the character meaning its children
          contain body parts.
        
*/

using UnityEngine;
using System.Collections;

// for physics and collision
[RequireComponent(typeof(Rigidbody))]

// for collision
[RequireComponent(typeof(Collider))]

public class BodyPart : Item
{
    // Physics components
    // hidding base memebers since these properties are depricated
    private new Rigidbody rigidbody;
    private new Collider collider;

    // Required parts?
    //int importance;

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        // ignore collision with parent to prevent flying when detached
        if (transform.parent != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());
    }

    // Use this for initialization
    void Start ()
    {
    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    /// <summary>
    /// Set this body part to active, this means it is
    /// </summary>
    public void SetActive()
    {
        // turn off kinematics so its physicsy
        rigidbody.isKinematic = false;
    }
}
