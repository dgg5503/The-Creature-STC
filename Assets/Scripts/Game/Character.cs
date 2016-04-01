﻿/*
    Abstract Character Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
        + CUT COLLIDER IN HALF WHEN NO LEGS
        + IGNORE CAPSULE COLLIDER WHEN DETACHING
        + MAKE DEAD GUY KINEMATIC

        + PICK UP AND CARRY B PARTS (HIT E)
        + CLICK ON BODY PART TO DETACH (FOR PLAYER)
        + CLICK ON BODY PART OF DEAD TO DETACH (when close)

        - is grounded
    NOTES
        - Attach this script to the root of the character meaning its children
          contain body parts.
*/

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Inventory))]
public abstract class Character : MonoBehaviour
{
    // Body stuff
    //protected List<BodyPart> bodyParts;
    protected Dictionary<int, CustomJoint> joints;
    protected List<RegularItem> regularItems;
    protected BodyPart root;

    // Inventory
    protected Inventory inventory;

    // Stats
    protected int defense;

    // Movement n' Physics
    protected float accelerationScalar;
    protected float rotationAccelFactor;
    protected float maxSpeed;
    protected Vector3 acceleration;
    private Vector3 velocity;
    protected new Rigidbody rigidbody;
    protected new CapsuleCollider collider;

    // States n' Actions
    private bool isAlive;

    /// <summary>
    /// Get the inventory of this character.
    /// </summary>
    public Inventory Inventory { get { return inventory; } }

    protected bool IsAlive
    {
        get
        {
            return isAlive;
        }

        set
        {
            isAlive = value;
            
            if (isAlive == false)
            {
                //root.Detach();
                // set all bparts to false kinematics...
                BodyPart[] allBodyParts = GetComponentsInChildren<BodyPart>();
                for (int i = 0; i < allBodyParts.Length; ++i)
                    allBodyParts[i].SetSilly();
            }
        }
    }

    /*
    public bool IsGrounded
    {
        get
        {
            return Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y);
        }
    }
    */

    // actions go here...

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected virtual void Awake()
    {
        // Grab all children with BodyPart scripts and store them for reference
        // Q: Is the head/torso a body part with inf health?
        
        joints = GetComponentsInChildren<CustomJoint>().ToDictionary(x => x.JointType, x => x);
        foreach (CustomJoint joint in joints.Values)
        {
            Debug.Log(string.Format("{0}: {1}", joint.JointType, joint.name));
        }

        // get all body parts that are children and set joint relationships ONCE
        // all other joint relationship will be handled in attach and detach

        // get root and set its skeleton
        foreach (Transform possBodyPart in transform)
            if ((root = possBodyPart.GetComponent<BodyPart>()) != null)
                break;

        root.InitSkeleton(joints);

        // init items in hand
        regularItems = new List<RegularItem>();

        // init this characters inventory
        inventory = GetComponent<Inventory>();

        // init defense modifier
        defense = 10;

        // init physics defaults
        accelerationScalar = 1.25f;
        rotationAccelFactor = .01f;
        maxSpeed = 2.0f;
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
        // get the overall capsule collider
        collider = GetComponent<CapsuleCollider>();

        // calc bounds for the character
        //StartCoroutine(RecalculateCollisionBounds());
        RecalculateCollisionBounds();

        // always start alive
        isAlive = true;

        // set layer of character colliders to 10
        gameObject.layer = 10;
    }

	// Use this for initialization
	void Start ()
    {
        
    }

    protected virtual void Update ()
    {
       
    }

    void FixedUpdate()
    {
        // calculate movement
        ProcessMovement();

        // apply velocity changes to this character
        if (acceleration != Vector3.zero)
        {
            // TODO: When on slope, apply velocity parallel to slope with slope max.?
            velocity += acceleration * Time.deltaTime;

            // lerp directional velocity from this objects forward to the accel norm
            // creates rotation effect purely from velocity
            // TODO: looks smooth when not performing a 180
            //float forwardAngle = Mathf.Atan2(transform.forward.x, transform.forward.z);
            //float accelAngle = Mathf.Atan2(acceleration.normalized.x, acceleration.normalized.z);
            //float angleBetween = Vector3.Angle(transform.forward, acceleration.normalized) * Mathf.Deg2Rad;
            //Vector3 facingDir = transform.forward;
            //Vector3 desiredDir = acceleration.normalized;

            //rotate velocity based on turnspeed uniformly
            //float speed = 10 * Time.deltaTime;
            //velocity = Vector3.RotateTowards(transform.forward, acceleration.normalized, rotationAccelFactor * (angleBetween / Mathf.PI) * Time.deltaTime, 0) * velocity.magnitude;

            velocity = Vector3.LerpUnclamped(transform.forward, acceleration.normalized, Time.deltaTime * rotationAccelFactor) * velocity.magnitude;

            //Debug.Log(string.Format("{0}, {1} : {2}", transform.forward, acceleration.normalized, Time.deltaTime * rotationAccelFactor));

            //Debug.Log(string.Format("FWD: {0} ACC: {1}", forwardAngle, accelAngle));

            //Debug.DrawLine(transform.position, transform.position + (transform.forward + acceleration.normalized) * 10.0f, Color.black);
        }
        else
        {
            // not sure if this is accurate???
            velocity = Vector3.LerpUnclamped(velocity, Vector3.zero, Time.deltaTime * accelerationScalar);

            // decrease veloctiy if no acceleration
            // TODO: base this on friction of surface?
            // deaccel until velocity is oppposite dir of forward
            // opposite dir of stopping forward.
            /*
            if (velocity.magnitude > .00001)
            {
                //Debug.Log(velocity.magnitude);
                //Debug.DrawLine(transform.position, transform.position + velocity.normalized * 10, Color.black);
                //Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue);
                velocity *= (1 / accelerationScalar) * Time.deltaTime;
                //velocity += transform.forward * -accelerationScalar * Time.deltaTime;
            }
            else
            {
                
                velocity = Vector3.zero;
            }
            // */
        }

        // clamp to this characters max velocity.
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // makes character face velocity.
        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
            //transform.forward = Vector3.LerpUnclamped(transform.forward, velocity.normalized, Time.deltaTime * (accelerationScalar * 2));
        }


        // move the character based on velocity
        //transform.position += (velocity * Time.deltaTime);

        // TODO: perhaps use addforce to rid of this check?
        // or just use the RB velocity overall.

        // TMP FIX
        //velocity.y = rigidbody.velocity.y;
        if (velocity != Vector3.zero)
            rigidbody.velocity = velocity;


        // set accel to 0
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Use this function for character specific movement.
    /// i.e. player is controlled via input but AI will move based on rules.
    /// </summary>
    abstract protected void ProcessMovement();

    
    public bool Attach(BodyPart bodyPartToAttach)
    {
        if (bodyPartToAttach == null || !joints.ContainsKey(bodyPartToAttach.BodyPartType))
            return false;

        if (!bodyPartToAttach.SetSkeleton(joints))
            return false;

        RecalculateCollisionBounds();
        return true;
    }
    

    /// <summary>
    /// Detaches a body part based on its index within the list of body parts.
    /// </summary>
    /// <param name="bodyPart">Index location of the body part to detach.</param>
    /// <returns>Reference to body part detached if it exists.</returns>
    public BodyPart Detach(int bodyPartID)
    {
        // see if part exists
        if (!joints.ContainsKey(bodyPartID))
        {
            Debug.Log("Failed: " + joints[bodyPartID].name);
            return null;
        }

        // store in a variable
        BodyPart tmpPart = joints[bodyPartID].BodyPart;

        if (tmpPart == null)
            return null;

        // set as active in the world.
        tmpPart.Detach();

        // adjust capsule collider pivot to new center and height
        RecalculateCollisionBounds();

        return tmpPart;
    }

    /// <summary>
    /// Recalculates the capsule colliders bounds.
    /// TODO: IGNORE HANDS
    /// </summary>
    private void RecalculateCollisionBounds()
    {
        // preserve rotation, rotate to 0 and then put back.
        //Quaternion currLocalRot = transform.localRotation;
        
        //transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        // RECURSIVELY GO THROUGH ALL CHILDREN AND ENCAPSULATE
        // TODO: make this function better

        // Get first body part (doesnt matter which one...)
        Bounds initialBounds = root.GetComponent<MeshFilter>().mesh.bounds;

        RecalculateCollisionBounds(ref initialBounds, root.gameObject);

        collider.center = initialBounds.center;
        collider.radius = Mathf.Max(initialBounds.size.x / 2, initialBounds.size.z / 2);
        collider.height = initialBounds.size.y;

        // put back rotation
        //transform.localRotation = currLocalRot;
    }

    // still broken....
    // TODO, FIX STRANGE ANOMLY WHERE BODY PARTS SLOWLY MOVE AWAY FROM ORIGIN LOCATIONS.
    // TODO, MAKE SURE THIS IS CALLED IN FIXED UPDATE (BEFORE PHYSICS ARE CALCd)
    // PERHAPS VELOCITY JUST NEEDS TO BE IN FIXED UPDATE?
    // REIMPLEMENT ROTATION TO IDENTITY (OR DEFAULT POSE)
    // YIELD FIXED UPDATE?
    // TODO, ALERRRRRRRT MAKE SURE TO PUT IN CHECKS FOR WHEN RESIZING UP/DOWN, DONT WANNA GO OOB :)))
    private void RecalculateCollisionBounds(ref Bounds currentBounds, GameObject currentBodyPart)
    {
        // QUICK FIX TODO DELETE
        BodyPart currBPart = currentBodyPart.GetComponent<BodyPart>();
        if (currBPart != null && (currBPart.BodyPartType == 1 || currBPart.BodyPartType == 5))
            return;

        // keep last local rot
        Quaternion currLocalRot = currentBodyPart.transform.localRotation;
        currentBodyPart.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (currentBodyPart.transform.childCount != 0)
            foreach (Transform child in currentBodyPart.transform) 
                RecalculateCollisionBounds(ref currentBounds, child.gameObject);

        // attempt to get MeshFilter
        // TODO OR ASK FOR MESHFILTER IN FUNCTION AS WE CAN DO MESHFILTER.TRANSFORM
        MeshFilter tmpMeshFilter = currentBodyPart.GetComponent<MeshFilter>();
        if (tmpMeshFilter == null)
            return;

        Bounds newBounds = currentBodyPart.GetComponent<MeshFilter>().mesh.bounds;

        // GET CENTER OF MESH
        // DIV BY LOCAL SCALE TO MOVE CENTER TO APPROPRIATE POSITION...
        // !!!!!! THIS IS WHY EVERYTHING SHOULD IMPORT AT 1 1 1 SCALE !!!!!!!
        // RENDERER CENTER BOUNDS ARE LOCAL
        // FIND A WAY TO GET TRUE CENTER ANOTHER WAY (this is because it is in world space and NOT local.
        //newBounds.center = transform.InverseTransformPoint(currentBodyPart.transform.TransformPoint(newBounds.center));
        newBounds.center = transform.InverseTransformPoint(currentBodyPart.transform.TransformPoint(newBounds.center));
        currentBounds.Encapsulate(newBounds);

        // put back rotation after all have been processed?
        currentBodyPart.transform.localRotation = currLocalRot;
    }
}