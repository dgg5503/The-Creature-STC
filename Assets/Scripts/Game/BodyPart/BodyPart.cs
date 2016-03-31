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
    private CustomJoint joint;

    [SerializeField]
    private Vector3 initialLocalPosition = Vector3.zero; // holds inital rotation and position relative to parent

    [SerializeField]
    private Quaternion initialLocalRotation = Quaternion.identity;

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
            if (bodyPartType == -1)
                bodyPartType = value;
            else
                Debug.Log("CANT SET BODY PART TYPE AFTER ALREADY SET");
        }
    }

    // DO NOT CHANGE THIS!!
    public Vector3 InitialLocalPosition
    {
        get
        {
            return initialLocalPosition;
        }

        set
        {

            initialLocalPosition = value;
        }
    }

    // DO NOT CHANGE THIS!
    public Quaternion InitialLocalRotation
    {
        get
        {
            return initialLocalRotation;
        }

        set
        {
            initialLocalRotation = value;
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
    public CustomJoint Joint
    {
        get
        {
            return joint;
        }

        set
        {
            if (joint == null || value.JointType == bodyPartType)
            {
                joint = value;
            }
            else
                Debug.Log("ERROR: Trying to attach " + name +
                    " to JointType " + value.JointType + " but its " + bodyPartType);
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
        //joint = GetComponentInParent<CustomJoint>();

        // Not connected
        if (joint == null)
        {
            // start rb as loose on the ground
            rigidbody.isKinematic = false;
        }
        else
        {
            // doing this here since rigidbodies are created on parents at awake.
            Collider parentCollider;
            if (transform.parent != null &&
                (parentCollider = transform.parent.GetComponent<Collider>()) != null)
            {
                // ignore collision with parent to prevent spazzing of merged bparts
                Physics.IgnoreCollision(collider, parentCollider);
                Debug.Log(string.Format("{0} ignores coll with {1}", transform.parent.name, name));
            }
        }

        // TODO ignore collision with all immediate children no matter type
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (joint != null)
            transform.localRotation = joint.transform.localRotation;

        //Debug.Log(string.Format("{0}", expectedParentType));
    }


    public bool SetParent(CustomJoint parentJoint)
    {
        // TO-DO: SETPARENT AND ATTACH FUNCTION SHOULD LOOP THROUGH ALL BPARTS CONNECTED TO THIS BPART AND SET THEIR JOINTS

        //Debug.Log(string.Format("{0} expecting {1} and got {2}", name, expectedParentType, parent.name));
        // Make sure we're not already attached to anything
        // Also make sure that parent is the expected type
        if (parentJoint == null || parentJoint.JointType != bodyPartType || transform.parent != null)
            return false;

        // destroy hinge component if it exists
        Destroy(GetComponent<HingeJoint>());

        // set this joint to new parent joint
        joint = parentJoint;

        // recurse through children and connect.

        // parent it
        transform.parent = parentJoint.transform;

        // set back to kinematic
        rigidbody.isKinematic = true;

        // set our parent, localposition, rotation based on offset value of subJoint
        transform.localRotation = initialLocalRotation;

        // PERHAPS SUBJOINT TO PIV - BODY LENGTH?
        transform.localPosition = initialLocalPosition;
        //transform.localPosition = initialLocalTransform.position * (bodyPartLength / parentJoint.OffsetProportion);

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
        joint = null;
        rigidbody.isKinematic = false;
        transform.parent = null;

        // reapply collision detection to parent
        if (Parent != null)
            Physics.IgnoreCollision(collider, Parent.GetComponent<Collider>(), false);

        // recursively go down joint path (starting with this joint) and call detach and attach to this bodypart.
        DetachAndParent(joint);

        // turn off kinematic
        rigidbody.isKinematic = false;

        // unparent / DEREFERENCE FROM SUBJOINT
        transform.parent = null;
    }

    private void DetachAndParent(CustomJoint joint)
    {
        // if no bodypart, then we're done
        if (joint.BodyPart == null)
            return;

        // see what next attached body part is from this joint
        CustomJoint nextJoint = null;

        // get immediate children
        foreach (Transform child in joint.transform)
            if ((nextJoint = child.GetComponent<CustomJoint>()) != null)
                joint.BodyPart.DetachAndParent(nextJoint);

        if (joint.BodyPart == this)
            return;

        HingeJoint hinge = joint.BodyPart.gameObject.AddComponent<HingeJoint>();
        hinge.connectedBody = rigidbody;
        hinge.anchor = joint.BodyPart.transform.localPosition;

        // parent body part to this one
        joint.BodyPart.rigidbody.isKinematic = false;
        joint.BodyPart.transform.parent = transform;
    }

    private void AttachAndParent(CustomJoint joint)
    {
        // if no bodypart, then we're done
        if (joint.BodyPart != null)
            return;

        // see what next attached body part is from this joint
        CustomJoint nextJoint = null;

        // get immediate children
        foreach (Transform child in joint.transform)
            if ((nextJoint = child.GetComponent<CustomJoint>()) != null)
                joint.BodyPart.DetachAndParent(nextJoint);

        if (joint.BodyPart == this)
            return;

        Destroy(GetComponent<HingeJoint>());

        // parent body part to this one
        this.joint = joint;
        transform.parent = joint.transform;
        rigidbody.isKinematic = true;
        //joint.BodyPart.transform.parent = transform;
    }
}
