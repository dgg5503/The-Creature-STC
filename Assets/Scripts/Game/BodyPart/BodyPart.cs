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
using System.Collections;
using System;

// for physics and collision
[RequireComponent(typeof(Rigidbody))]

// for collision
[RequireComponent(typeof(Collider))]

public class BodyPart : Item, ISerializationCallbackReceiver
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

    // Serialized to store body part type.
    [SerializeField]
    private int bodyPartType = -1; // body part type of this body part based on ORIGINAL joint. (treated as an "enum")

    [SerializeField]
    private bool isDetachable = true;

    // Holds joint bodypart endpoint connections
    [Header("End Points")]
    public List<int> _keys = new List<int>();
    public List<Vector3> _values = new List<Vector3>();
    public Dictionary<int, Vector3> endPoints = new Dictionary<int, Vector3>();

    /// <summary>
    /// Get the body part type integer of this body part. Body part types should always matched with their
    /// attached joint.
    /// </summary>
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
    /*
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
    */

    // DO NOT CHANGE THIS!
    /*
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
    */

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
    /*
    public CustomJoint Joint
    {
        get
        {
            return joint;
        }

        set
        {
            if (value.JointType == bodyPartType)
            {
                joint = value;
                joint.BodyPart = this;

                if (joint.Parent != null)
                {
                    Debug.Log(joint.Parent.BodyPart.name);
                    SetParent(joint.Parent.BodyPart);
                }
            }
            else if(value == null)
            {
                joint = null;
                joint.BodyPart = null;
                Detach();
            }
            else
            {
                Debug.Log("ERROR: Trying to attach " + name +
                    " to JointType " + value.JointType + " but its " + bodyPartType);
            }
        }
    }
    */

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
        //bodyPartLength = GetComponent<MeshFilter>().mesh.bounds.max.magnitude * 2;
    }

    // Use this for initialization
    void Start ()
    {
        // no joint connected
        if (joint == null)
        {
            // start rb as loose on the ground
            rigidbody.isKinematic = false;

            // attach hinges if any children
            SetupPhysicsJoint();
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

    void FixedUpdate()
    {
        //Debug.Log("THE FIXED");
    }

    /// <summary>
    /// Late update used to update rotation from skeleton.
    /// LateUpdate occurs after Unity's internal animation.
    /// </summary>
    void LateUpdate()
    {
        if (joint != null)
            transform.localRotation = joint.transform.localRotation;
    }

    // FIX THIS
    public bool AttachTo(BodyPart parentBodyPart)
    {
        // Detach if connected to anything.
        //if (joint != null || transform.parent != null)
        //    Detach();

        //Debug.Log(string.Format("{0} expecting {1} and got {2}", name, expectedParentType, parent.name));
        // null check and ensure this body part has no parent and ensure correct type
        if (parentBodyPart == null || transform.parent != null)
            return false;



        // set this joint to new parent joint
        // ???
        /*
        if (associatedJoint != null)
        {
            joint = associatedJoint;
            joint.BodyPart = this;
        }
        */
        Debug.Log(name + " TO " + parentBodyPart.name);

        // parent it
        transform.parent = parentBodyPart.transform;

        // Get all child body parts
        BodyPart[] childBodyParts = transform.GetComponentsInChildren<BodyPart>();

        // Apply detach properties to all.
        for (int i = 0; i < childBodyParts.Length; ++i)
        {
            if (childBodyParts[i].isDetachable == true)
            {
                childBodyParts[i].SetParent(childBodyParts[i]);

                /*
                // add a hinge joint for visual indication that connection still exists
                // if parent body part has a joint, then it means its being animated
                // must delete then!
                if (childBodyParts[i] != this)
                    if ((tmpHinge = GetComponent<ConfigurableJoint>()) != null && parentBodyPart.joint != null)
                        Destroy(GetComponent<ConfigurableJoint>());

                // set back to kinematic
                rigidbody.isKinematic = true;

                // set our parent, localposition, rotation based on offset value of subJoint
                transform.localRotation = Quaternion.identity;

                // below should never error
                //Debug.Log
                transform.localPosition = parentBodyPart.endPoints[bodyPartType];

                //transform.localPosition = initialLocalTransform.position * (bodyPartLength / parentJoint.OffsetProportion);

                // re-ignore parent collision
                if (transform.parent != null)
                    Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());
                    */
            }
        }
        return true;
    }

    public bool SetParent(BodyPart newParent)
    {
        // cant set this as parent
        if (newParent == this)
            return false;

        // set parent
        transform.parent = newParent.transform;

        // add a hinge joint for visual indication that connection still exists
        // if parent body part has a joint, then it means its being animated
        // must delete then!
        ConfigurableJoint tmpHinge;
        if ((tmpHinge = GetComponent<ConfigurableJoint>()) != null && newParent.joint != null)
                Destroy(GetComponent<ConfigurableJoint>());

        // set back to kinematic
        if(rigidbody != null)
            rigidbody.isKinematic = true;

        // set our parent, localposition, rotation based on offset value of subJoint
        if (newParent.joint != null)
            transform.localRotation = newParent.joint.transform.localRotation;
        else
            transform.localRotation = Quaternion.identity;
        
        // below should never error
     
        transform.localPosition = newParent.endPoints[bodyPartType];

        //transform.localPosition = initialLocalTransform.position * (bodyPartLength / parentJoint.OffsetProportion);

        // re-ignore parent collision
        if (transform.parent != null && collider != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());

        return true;
    }

    // TO-DO: make recursive
    public bool SetSkeleton(Dictionary<int, CustomJoint> skeleton)
    {
        // null check
        if (skeleton == null || !skeleton.ContainsKey(bodyPartType))
            return false;

        // attach this body part to root, set joint and its bpart
        //joint = skeleton[bodyPartType];
        //joint.BodyPart = this;

        //if (joint.Parent != null)
        //{
        //    transform.parent = joint.Parent.BodyPart.transform;
        //}

        // Get all child body parts
        // TO-DO: CONSOLIDATE BOTH FORT LOOPS 
        BodyPart[] childBodyParts = transform.GetComponentsInChildren<BodyPart>();

        // Apply detach properties to all.
        for (int i = 0; i < childBodyParts.Length; ++i)
        {
            // if not in skeleton, ignore!
            if (childBodyParts[i].isDetachable == true &&
                skeleton.ContainsKey(childBodyParts[i].bodyPartType))
            {
                childBodyParts[i].joint = skeleton[childBodyParts[i].bodyPartType];
                childBodyParts[i].joint.BodyPart = childBodyParts[i];

                if(childBodyParts[i].joint.Parent != null)
                    childBodyParts[i].SetParent(childBodyParts[i].joint.Parent.BodyPart);

                // add a hinge joint for visual indication that connection still exists
                // if parent body part has a joint, then it means its being animated
                // must delete then!
                /*
                if ((tmpHinge = childBodyParts[i].GetComponent<ConfigurableJoint>()) != null)
                    Destroy(tmpHinge);

                // set off kinematic
                if(childBodyParts[i].rigidbody != null)
                    childBodyParts[i].rigidbody.isKinematic = false;

                // set our parent, localposition, rotation based on offset value of subJoint
                childBodyParts[i].transform.localRotation = childBodyParts[i].joint.transform.localRotation;

                // below should never error
                Debug.Log(childBodyParts[i].name + " -> " + childBodyParts[i].joint.name);
                Debug.Log("Gonna try " + childBodyParts[i].joint.Parent.name);
                childBodyParts[i].transform.localPosition = childBodyParts[i].joint.Parent.BodyPart.endPoints[childBodyParts[i].bodyPartType];

                // re-ignore parent collision
                if(childBodyParts[i].collider != null)
                    Physics.IgnoreCollision(childBodyParts[i].collider, childBodyParts[i].transform.parent.GetComponent<Collider>());
                    */
            }
        }
        return true;        
    }



    /// <summary>
    /// Detaches this body part from whatever parent it's connected to.
    /// TO-DO: Figure out why velocity doesnt carry over to joint?
    /// TO-DO: Perhaps make it so that every body part already has a joint!
    /// </summary>
    public BodyPart Detach()
    {
        //yield return new WaitForFixedUpdate();
        //Debug.Log("IN FIXED");
        if (isDetachable == false)
            return null;
            //yield break;

        // reapply collision detection to parent
        if (transform.parent != null)
        {
            Debug.Log("Enabled col between " + collider.name + " and " + transform.parent.name);
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>(), false);
        }

        // unparent THIS ONLY! (this is considered to be the root of the detached bodypart)
        transform.parent = null;

        // Get all child body parts
        BodyPart[] childBodyParts = transform.GetComponentsInChildren<BodyPart>();

        // Apply detach properties to all.
        for(int i = 0; i < childBodyParts.Length; ++i)
        {
            if (childBodyParts[i].isDetachable == true)
            {
                // ensure body part connection is off before detach
                if(childBodyParts[i].joint != null)
                    childBodyParts[i].joint.BodyPart = null;

                // unparent from joint, animation now off.
                childBodyParts[i].joint = null;

                // add a hinge joint for visual indication that connection still exists
                if (childBodyParts[i] != this)
                    childBodyParts[i].SetupPhysicsJoint();

                // turn off kinematic
                childBodyParts[i].rigidbody.isKinematic = false;
            }
        }

        return this;
    }

    /// <summary>
    /// Creates a hinge joint on this body part and attaches it to the current parent.
    /// </summary>
    private void SetupPhysicsJoint()
    {
        if (transform.parent != null)
        {
            // TO-DO: need max/min angle for hinge joint
            // TO-DO: need axis for hinge joint
            ConfigurableJoint hinge = gameObject.AddComponent<ConfigurableJoint>();
            hinge.connectedBody = transform.parent.GetComponent<Rigidbody>();
            hinge.xMotion = ConfigurableJointMotion.Locked;
            hinge.yMotion = ConfigurableJointMotion.Locked;
            hinge.zMotion = ConfigurableJointMotion.Locked;
            hinge.angularXMotion = ConfigurableJointMotion.Free;
            hinge.angularYMotion = ConfigurableJointMotion.Free;
            hinge.angularZMotion = ConfigurableJointMotion.Free;
            hinge.anchor = Vector3.zero;
        }
    }

    /*
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
    }
    */

    // --- EDITOR ONLY ---
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var kvp in endPoints)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        endPoints.Clear();
        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            endPoints.Add(_keys[i], _values[i]);
    }
}
