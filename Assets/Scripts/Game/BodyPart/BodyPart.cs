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

    // Serialized to store body part type.
    [SerializeField]
    private int bodyPartType = -1; // body part type of this body part based on ORIGINAL joint. (treated as an "enum")

    [SerializeField]
    private bool isDetachable = true;

    private bool isControlledByJoint;

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

    public CustomJoint Joint
    {
        get
        {
            return joint;
        }

        private set
        {
            if (value != null)
            {
                joint = value;
                joint.BodyPart = this;
                isControlledByJoint = true;
            }
            else
            {
                if (joint != null)
                {
                    joint.BodyPart = null;
                    joint = null;
                    isControlledByJoint = false;
                }
            }
        }
    }

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
            if (currHealth <= minHealth)
                Detach();
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

        // Control determined later
        isControlledByJoint = true;
    }

    // Use this for initialization
    void Start()
    {
        // no joint connected
        if (joint == null)
        {
            Debug.Log(name + " is null.");

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

    /// <summary>
    /// Late update used to update rotation from skeleton.
    /// LateUpdate occurs after Unity's internal animation.
    /// </summary>
    void LateUpdate()
    {
        if (joint != null && isControlledByJoint)
            transform.localRotation = joint.transform.localRotation;
    }

    public bool SetParent(BodyPart newParent)
    {
        // cant set this as parent
        if (newParent == this || newParent == null)
            return false;

        // set parent
        transform.parent = newParent.transform;

        // add a hinge joint for visual indication that connection still exists
        // if parent body part has a joint, then it means its being animated
        // must delete then!
        ConfigurableJoint tmpHinge;
        if ((tmpHinge = GetComponent<ConfigurableJoint>()) != null && newParent.Joint != null)
                Destroy(tmpHinge);

        // set back to kinematic
        if(rigidbody != null)
            rigidbody.isKinematic = true;

        // set our parent, localposition, rotation based on offset value of subJoint
        if (newParent.Joint != null)
            transform.localRotation = newParent.Joint.transform.localRotation;
        else
            transform.localRotation = Quaternion.identity;
        
        // below should never error
        transform.localPosition = newParent.endPoints[bodyPartType];

        // re-ignore parent collision
        if (transform.parent != null && collider != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());

        return true;
    }

    // TODO: FIND FIX FOR THIS FUNCTION, dont have 2
    public bool InitSkeleton(Dictionary<int, CustomJoint> skeleton)
    {
        // null check
        if (skeleton == null || !skeleton.ContainsKey(bodyPartType))
            return false;

        // Get all child body parts
        BodyPart[] childBodyParts = transform.GetComponentsInChildren<BodyPart>();

        // Apply detach properties to all.
        for (int i = 0; i < childBodyParts.Length; ++i)
        {
            // if not in skeleton, ignore!
            // childBodyParts[i].isDetachable == true &&
            if (skeleton.ContainsKey(childBodyParts[i].bodyPartType))
            {
                childBodyParts[i].Joint = skeleton[childBodyParts[i].bodyPartType];

                if (childBodyParts[i].Joint.Parent != null)
                    childBodyParts[i].SetParent(childBodyParts[i].Joint.Parent.BodyPart);
            }
        }
        return true;
    }

    // TO-DO: make recursive
    public bool SetSkeleton(Dictionary<int, CustomJoint> skeleton)
    {
        // null check, ensure type is in skeleton, dont attach if joint is full.
        if (skeleton == null || !skeleton.ContainsKey(bodyPartType) || skeleton[bodyPartType].BodyPart != null)
            return false;

        // ensure expected parent exists
        if (skeleton[bodyPartType].Parent.BodyPart == null)
            return false;

        // Get all child body parts
        BodyPart[] childBodyParts = transform.GetComponentsInChildren<BodyPart>();

        // Apply detach properties to all.
        for (int i = 0; i < childBodyParts.Length; ++i)
        {
            // if not in skeleton, ignore!
            // childBodyParts[i].isDetachable == true &&
            if (skeleton.ContainsKey(childBodyParts[i].bodyPartType))
            {
                childBodyParts[i].Joint = skeleton[childBodyParts[i].bodyPartType];

                if (childBodyParts[i].Joint.Parent != null)
                    childBodyParts[i].SetParent(childBodyParts[i].Joint.Parent.BodyPart);
            }
        }
        return true;        
    }

    /// <summary>
    /// Set this body part to "limp" but still connected.
    /// </summary>
    public void SetSilly()
    {
        SetupPhysicsJoint();
        Debug.Log("Set silly: " + name);
        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;
            Debug.Log("Set silly OFF: " + name);
        }

        isControlledByJoint = false;
        
    }
    /// <summary>
    /// Detaches this body part from whatever parent it's connected to.
    /// TO-DO: Figure out why velocity doesnt carry over to joint?
    /// TO-DO: Perhaps make it so that every body part already has a joint!
    /// </summary>
    public BodyPart Detach()
    {
        if (isDetachable == false || transform.parent == null)
            return null;

        // reapply collision detection to parent
        Collider parentCollider;
        if (transform.parent != null &&
            collider != null &&
            (parentCollider = transform.parent.GetComponent<Collider>()) != null)
            Physics.IgnoreCollision(collider, parentCollider, false);

        ConfigurableJoint tmpHinge;
        if ((tmpHinge = GetComponent<ConfigurableJoint>()) != null)
            Destroy(tmpHinge);

        // unparent THIS ONLY! (this is considered to be the root of the detached bodypart)
        transform.parent = null;

        // Get all child body parts
        BodyPart[] childBodyParts = transform.GetComponentsInChildren<BodyPart>();

        // Apply detach properties to all.
        for(int i = 0; i < childBodyParts.Length; ++i)
        {
            if (childBodyParts[i].isDetachable == true)
            {
                childBodyParts[i].Joint = null;

                // add a hinge joint for visual indication that connection still exists
                if (childBodyParts[i] != this)
                    childBodyParts[i].SetupPhysicsJoint();

                // turn off kinematic
                if(childBodyParts[i].rigidbody != null)
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
