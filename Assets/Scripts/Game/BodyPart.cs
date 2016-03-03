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

    //int importance;

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected override void Awake()
    {
        base.Awake();
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        expectedParentType = "";
        hinge = null;

        // if not already tagged as bodypart, force tag as BodyPart
        if (tag != "BodyPart")
            tag = "BodyPart";

        // if this body part is "root," set to kinematic
        // else set to false ?
        // TODO: should setting to kinematic be done in CHARACTER since 
        // we could have a ragdoll on the ground with no collider?
        if (name == "root")
            rigidbody.isKinematic = true;

        // if parent exists, set desired name
        if (transform.parent != null)
            expectedParentType = transform.parent.name;

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
                // ignore collision with parent to prevent spazzing of merged bparts
                Physics.IgnoreCollision(collider, parentBodyPart.GetComponent<Collider>());

                // create hinge
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
    /// TODO REMOVE THIS MAYBE?
    /// QUICK AND DIRTY REMOVAL OF COLLISION BETWEEN ALL BPARTS AT ALL TIMES.
    /// POSSIBLY BETTER TO DO THIS AT AWAKE AND DURING LOADS???
    /// </summary>
    /// <param name="collision"></param>
    /*
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BodyPart")
        {
            Debug.Log(collision.gameObject.name);
            Physics.IgnoreCollision(collision.collider, collider);
        }
    }
    */

    /// <summary>
    /// This will set the body part to connected.
    /// </summary>
    public bool SetStatic(BodyPart parent, Vector3 position, Quaternion rotation)
    {
        //Debug.Log(string.Format("{0} expecting {1} and got {2}", name, expectedParentType, parent.name));
        if (transform.parent != null || parent.name != expectedParentType)
            return false;

        //Debug.Log(string.Format("{0} attached to {1}", name, parent.name));

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

        // re-ignore parent collision
        if (transform.parent != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>());
        return true;
    }

    /// <summary>
    /// Set this body part to active, this means it is in the environment.
    /// </summary>
    public void SetActive()
    {
        // reapply collision detection to parent
        if (transform.parent != null)
            Physics.IgnoreCollision(collider, transform.parent.GetComponent<Collider>(), false);

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
