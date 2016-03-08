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
    // Static
    private static Dictionary<int, string> bodyPartTypeDictionary = new Dictionary<int, string>();

    // Physics components
    // hidding base memebers since these properties are depricated
    private new Rigidbody rigidbody;
    private new Collider collider;
    private HingeJoint hinge;

    // Part specific
    private PosRot initialLocalTransform; // holds inital rotation and position relative to parent
    private PosRot lastLocalTransform; // holds last local transform when attached to a parent
    private int bodyPartType; // body part type of this body part. (treated as an "enum")
    private int expectedParentType; // this should only be set once. (treated as an "enum")

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
    /// Gets the body part type of this body part as a string.
    /// </summary>
    public string BodyPartType
    {
        get
        {
            return bodyPartTypeDictionary[bodyPartType];
        }
    }

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected override void Awake()
    {
        base.Awake();

        // add this body part type to our list of bpart types
        Initialize();

        // setup physics sutff
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        hinge = null;

        // Initial local position and rotation relative to initial parent.
        initialLocalTransform = new PosRot(transform.localPosition, transform.localRotation);

        // zero out last local
        lastLocalTransform = new PosRot(Vector3.zero, Quaternion.identity);

        // if this body part is "root," set to kinematic
        // else set to false ?
        // TODO: should setting to kinematic be done in CHARACTER since 
        // we could have a ragdoll on the ground with no collider?
        if (name == "root")
            rigidbody.isKinematic = true;

        // health
        // TODO, DETERMINE WHEN TO SET THESE?
        maxHealth = 100;
        minHealth = 30;
        currHealth = maxHealth;
    }

    // Use this for initialization
    void Start ()
    {
        // create hinge to parent
        // doing this here since rigidbodies are created on parents at awake.
        if (transform.parent != null)
        {
            BodyPart parentBodyPart = transform.parent.GetComponent<BodyPart>();
            if (parentBodyPart != null)
            {
                // this can only be set in START because AWAKE is where we finalize body part types.
                expectedParentType = parentBodyPart.bodyPartType;

                // ignore collision with parent to prevent spazzing of merged bparts
                Physics.IgnoreCollision(collider, parentBodyPart.GetComponent<Collider>());

                // create hinge
                hinge = gameObject.AddComponent<HingeJoint>();
                hinge.connectedBody = parentBodyPart.rigidbody;
                hinge.anchor = transform.position;

                // TODO REMOVE QUICK NASTY HEAD CHECK
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

    /*
        Pass a skeleton and search for desired body part
            If the part has no children WHICH ARE ALSO BPARTS, then attach
    */
    /// <summary>
    /// Attaches this bodypart to a given parent body part.
    /// </summary>
    /// <param name="parent">Parent body part to attach to.</param>
    /// <returns>True - If attached successfully False- If failed to attach</returns>
    public bool AttachTo(BodyPart parent)
    {
        //Debug.Log(string.Format("{0} expecting {1} and got {2}", name, expectedParentType, parent.name));
        // Make sure we're not already attached to anything
        // Also make sure that parent is the expected type
        if (transform.parent != null || parent.bodyPartType != expectedParentType)
            return false;

        //Debug.Log(string.Format("{0} attached to {1}", name, parent.name));

        // set our parent, localposition, rotation.
        transform.parent = parent.transform;
        transform.localPosition = initialLocalTransform.position;
        transform.localRotation = initialLocalTransform.rotation;

        // reattach hinge
        hinge = gameObject.AddComponent<HingeJoint>();
        hinge.connectedBody = parent.rigidbody;
        hinge.anchor = transform.position;
        hinge.axis = Vector3.right;

        // TODO REMOVE QUICK NASTY HEAD CHECK
        if (name == "head")
            HeadCheck(hinge);

        // re-ignore parent collision
        if (transform.parent != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());

        return true;
    }

    /// <summary>
    /// Detaches this body part from whatever parent it's connected to.
    /// </summary>
    public void Detach()
    {
        // reapply collision detection to parent
        if (transform.parent != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>(), false);

        // unconnect hinge
        Destroy(GetComponent<HingeJoint>());

        // unparent
        transform.parent = null;
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

    /// <summary>
    /// Sets body part type and grabs expected parent body part type.
    /// Currently uses NAME of game object to define names of bodyparts.
    /// </summary>
    private void Initialize()
    { 
        // search dictionary for our name and set the int to bodyPartType
        foreach (KeyValuePair<int, string> kvp in bodyPartTypeDictionary)
        {
            if (kvp.Value == name)
            {
                bodyPartType = kvp.Key;
                return;
            }
        }

        // if we got here, means we didnt find our name in the dictionary, lets add it!
        bodyPartType = bodyPartTypeDictionary.Count;
        bodyPartTypeDictionary.Add(bodyPartTypeDictionary.Count, name);
    }
}
