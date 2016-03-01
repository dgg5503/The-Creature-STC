/*
    Body Part Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
        - Detach()
        - Attach(Character c)

    NOTES
        - Attach this script to the root of the character meaning its children
          contain body parts.
*/

using UnityEngine;
using System.Collections;

enum BodyPartType
{
    LEG,
    ARM,
    HAND,
    HEAD,
    TORSO
}

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
    private HingeJoint hinge;

    // Part specific 
    //private BodyPartType bodyPartType;
    //private BodyPartType expectedParentType;
    private string expectedParentType; // store expected parent type that should be there in order to reattach
    private int health;

    // Properties
    public int Health { get { return health; } }

    // Required parts?
    //int importance;

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected override void Awake()
    {
        //Debug.Log("CALLED");
        base.Awake();
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        expectedParentType = "";
        hinge = null;

        // if not already tagged, tag as BodyPart
        if (tag == "Untagged")
            tag = "BodyPart";

        // OLD: Collision is now ignored between ALL ITEMS
        // (all items on the items layer ignore items on the items layer)
        // ignore collision with parent to prevent flying when detached
        if (transform.parent != null)
        {
            expectedParentType = transform.parent.name;
            
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());
        }

        // health
        health = 100;
    }

    // Use this for initialization
    void Start ()
    {
        // create hinge to parent
        // doing this here since rigidbodies are created at awake.
        if (transform.parent != null)
        {
            
            BodyPart parentBodyPart = transform.parent.GetComponent<BodyPart>();
            if (parentBodyPart != null)
            {
                hinge = gameObject.AddComponent<HingeJoint>();
                hinge.connectedBody = parentBodyPart.rigidbody;
                hinge.anchor = transform.position;

                // QUICK NASTY HEAD CHECK
                if (name == "head")
                    HeadCheck(hinge);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(string.Format("{0}", expectedParentType));
    }

    /// <summary>
    /// This will set the body part to connected.
    /// </summary>
    public bool SetStatic(BodyPart parent, Vector3 position, Quaternion rotation)
    {
        //Debug.Log(string.Format("{0} = {1}?", parent.name, expectedParentType));
        if (transform.parent != null || parent.name != expectedParentType)
            return false;

        Debug.Log(string.Format("{0} attached to {1}", name, parent.name));

        // place on layer 9 since its an item
        gameObject.layer = 9;

        // set our parent, localposition, rotation.
        transform.parent = parent.transform;
        transform.localPosition = position;
        transform.localRotation = rotation;

        // reattach hinge
        hinge = gameObject.AddComponent<HingeJoint>();
        hinge.connectedBody = parent.rigidbody;
        hinge.anchor = transform.position;
        hinge.axis = Vector3.right;

        // QUICK NASTY HEAD CHECK
        if (name == "head")
            HeadCheck(hinge);

        // turn on kinematics
        //rigidbody.isKinematic = true;

        // ignore parent collision
        if (transform.parent != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());
        return true;
    }

    /// <summary>
    /// Set this body part to active, this means it is in the environment.
    /// </summary>
    public void SetActive()
    {
        // reapply collision
        if (transform.parent != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>(), false);

        // turn off kinematics so its physicsy
        //rigidbody.isKinematic = false;

        // apply collision to bparts again.
        // body part is now part of the environment (layer 8 is environment)
        gameObject.layer = 8;

        // unconnect hinge
        Destroy(GetComponent<HingeJoint>());
    }

    // apply limits to the head :)
    private void HeadCheck(HingeJoint hinge)
    {
        JointLimits limits = hinge.limits;
        limits.min = -20f;
        limits.max = 20f;
        hinge.limits = limits;
        hinge.axis = new Vector3(1, 1, 1);
        hinge.useLimits = true;
    }
}
