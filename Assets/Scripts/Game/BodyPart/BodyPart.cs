/*
    Body Part Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
        
    DONE
        + Detach(string nameOfBpart)
        + Attach(BodyPart bodyPart)

    NOTES
        - Attach this script to an empty game object that has the characters
          root body as a child. This is to simplify layer collision.
*/

using UnityEngine;
using System.Collections.Generic;

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

    // Part specific
    private Joint joint;
    private PosRot initialLocalTransform; // holds inital rotation and position relative to parent
    private float bodyPartLength;

    // Serialized to store body part type.
    [SerializeField]
    private int bodyPartType = -1; // body part type of this body part based on ORIGINAL joint. (treated as an "enum")

    public int BodyPartType
    {
        get
        {
            return bodyPartType;
        }

        set
        {
            bodyPartType = value;
        }
    }

    //private string expectedParentType; // store expected parent type that should be there in order to reattach

    // Stats
    private int currHealth;
    private int minHealth;
    private int maxHealth;

    // Properties
    /// <summary>
    /// Get or set the amount of health this body part has.
    /// If health is less than minHealth, it will detach.
    /// </summary>
    public int Health
    {
        get
        {
            return currHealth;
        }

        set
        {
            // TODO CHECK TO SEE IF SHOULD DETACH.

            currHealth = value;
        }
    }

    /// <summary>
    /// Get or set the minimum health value of this body part.
    /// When current health is lessthan or equal to min health, this body part will automatically detach.
    /// </summary>
    public int MinHealth
    {
        get
        {
            return minHealth;
        }

        set
        {
            // TODO CHECK TO SEE IF SHOULD DETACH.
            minHealth = value;
        }
    }

    /// <summary>
    /// Get or set the current joint that this body part is attached to.
    /// </summary>
    public Joint Joint
    {
        get
        {
            return joint;
        }
    }

    /// <summary>
    /// Returns this body part's parent bypassing joints.
    /// </summary>
    public BodyPart Parent
    {
        get
        {
            if(Joint.Parent != null)
                return Joint.Parent.BodyPart;
            return null;
        }
    }

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected override void Awake()
    {
        base.Awake();

        // setup physics sutff
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

        // all kinematic.
        rigidbody.isKinematic = true;

        // health        
        // TODO, DETERMINE WHEN TO SET THESE?
        maxHealth = 100;
        minHealth = 30;
        currHealth = maxHealth;

        // calculate length of this body part based on initial rotation.
        // grab extents and length based on default rotation
        bodyPartLength = GetComponent<MeshFilter>().mesh.bounds.max.magnitude * 2;
    }

    // Use this for initialization
    void Start ()
    {
        // Grab inital local transform info for THIS BODY PART
        //initialLocalTransform = GameManager.LocalPosRotDictonary[name];

        // Grab body part type from EXPECTED initial sub joint.
        //bodyPartType = initialLocalTransform.relativeSubJoint;

        // Apply actions to CURRENT joint information
        joint = GetComponentInParent<Joint>();

        // Not connected
        if (joint == null)
        {
            // start rb as loose on the ground
            rigidbody.isKinematic = false;
        }
        else
        {
            // doing this here since rigidbodies are created on parents at awake.
            if (joint.Parent != null)
            {
                // ignore collision with parent to prevent spazzing of merged bparts
                Physics.IgnoreCollision(collider, Parent.GetComponent<Collider>());
                Debug.Log(string.Format("{0} ignores coll with {1}", Parent.name, name));
            }
        }

        // TODO ignore collision with all immediate children no matter type
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(string.Format("{0}", expectedParentType));
    }


    public bool AttachTo(Joint parentJoint)
    {
        //Debug.Log(string.Format("{0} expecting {1} and got {2}", name, expectedParentType, parent.name));
        // Make sure we're not already attached to anythingian
        // Also make sure that parent is the expected type
        if (parentJoint != null
            || parentJoint.JointType != bodyPartType)
            return false;

        // set this parts subjoint.
        //subJoint = parentJoint.SubJoint;

        // parent it
        transform.parent = parentJoint.transform;

        // set back to kinematic
        rigidbody.isKinematic = true;

        // set our parent, localposition, rotation based on offset value of subJoint
        transform.localRotation = initialLocalTransform.rotation;
       
        // PERHAPS SUBJOINT TO PIV - BODY LENGTH?
        transform.localPosition = initialLocalTransform.position * (bodyPartLength / parentJoint.OffsetProportion);

        // re-ignore parent collision
        if (Parent != null)
            Physics.IgnoreCollision(collider, Parent.GetComponent<Collider>());

        return true;
    }
    

    /// <summary>
    /// Detaches this body part from whatever parent it's connected to.
    /// </summary>
    public void Detach()
    {
        // reapply collision detection to parent
        if (Parent != null)
            Physics.IgnoreCollision(collider, Parent.GetComponent<Collider>(), false);

        // turn off kinematic
        rigidbody.isKinematic = false;

        // set current subjoint to null
        //subJoint = null;

        // unparent / DEREFERENCE FROM SUBJOINT
        transform.parent = null;

    }
}
