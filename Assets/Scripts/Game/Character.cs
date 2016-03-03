/*
    Abstract Character Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
        - GRAB PARTS FROM DEAD GUY
        * CUT COLLIDER IN HALF WHEN NO LEGS (currently working on)
        + IGNORE CAPSULE COLLIDER WHEN DETACHING
        - PICK UP AND CARRY B PARTS
    NOTES
        - Attach this script to the root of the character meaning its children
          contain body parts.
*/

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Inventory))]
public abstract class Character : MonoBehaviour
{
    // Body stuff
    //protected List<BodyPart> bodyParts;
    protected Dictionary<string, BodyPart> bodyParts;
    protected List<RegularItem> regularItems;
    protected Dictionary<string, Vector3> bodyPartOrigins; // name_of_part : position

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
    protected new Collider collider;

    // States n' Actions
    protected bool isAlive;

    /// <summary>
    /// Get the dictionary of body parts for this character.
    /// </summary>
    public Dictionary<string, BodyPart> BodyParts { get { return bodyParts; } }

    /// <summary>
    /// Get the inventory of this character.
    /// </summary>
    public Inventory Inventory { get { return inventory; } }

    public bool IsGrounded
    {
        get; set;
    }

    // actions go here...

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected virtual void Awake()
    {
        Debug.Log(transform.rotation);
        // Grab all children with BodyPart scripts and store them for reference
        // Q: Is the head/torso a body part with inf health?

        //bodyParts = new List<BodyPart>(transform.GetComponentsInChildren<BodyPart>());
        bodyParts = transform.GetComponentsInChildren<BodyPart>().ToDictionary(x => x.name, x => x.GetComponent<BodyPart>());

        // remove root from list for now
        //bodyParts.Remove(GetComponent<BodyPart>());

        // init items in hand
        regularItems = new List<RegularItem>();

        // save all INITIAL body part locallocations

        bodyPartOrigins = new Dictionary<string, Vector3>();
        foreach(BodyPart bodyPart in bodyParts.Values)
            bodyPartOrigins.Add(bodyPart.name, bodyPart.transform.localPosition);

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
        collider = GetComponent<BoxCollider>();
        
        // calc bounds for the character
        RecalculateCollisionBounds();

        // always start alive
        isAlive = true;

        // set layer of character colliders to 10
        gameObject.layer = 10;
        Debug.Log(transform.rotation);
    }

	// Use this for initialization
	void Start ()
    {
	    
	}

    /*
    protected virtual void Update()
    {
        ProcessMovement();

        // apply velocity changes to this character
        if (acceleration != Vector3.zero)
            velocity += acceleration * Time.deltaTime;

        // local space velocity
        Vector3 localVelNorm = transform.InverseTransformDirection(velocity.normalized);

        // determine amount to turn
        float turnAmount = Mathf.Atan2(localVelNorm.x, localVelNorm.z);

        // turn based on z position (which is forward)
        float turnSpeed = Mathf.Lerp(180, 360, localVelNorm.z);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);

        // apply velocity to local coord
        //transform.localPosition.z += (velocity.z * Time.deltaTime);

        // set accel to 0
        acceleration = Vector3.zero;


    }*/

    // Update is called once per frame
    
    protected virtual void Update ()
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
        if(velocity != Vector3.zero)
            rigidbody.velocity = velocity;

        // set accel to 0
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Use this function for character specific movement.
    /// i.e. player is controlled via input but AI will move based on rules.
    /// </summary>
    abstract protected void ProcessMovement();

    /// <summary>
    /// Attaches a provided body part to its appropriate area if the parts
    /// parent exists on the character.
    /// </summary>
    /// <param name="bodyPartToAttach">Body part to attach.</param>
    /// <returns>True if the body part was attached. False if the parent part was not found.</returns>
    public bool Attach(BodyPart bodyPartToAttach)
    {
        // place body part in first available location
        foreach (BodyPart bp in bodyParts.Values)
        {
            if (bodyPartToAttach.SetStatic(bp, bodyPartOrigins[bodyPartToAttach.name], Quaternion.identity))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Detaches a body part based on its index within the list of body parts.
    /// </summary>
    /// <param name="bodyPart">Index location of the body part to detach.</param>
    /// <returns>Reference to body part detached if it exists.</returns>
    public BodyPart Detach(string bodyPartName)
    {
        // see if part exists
        if (!bodyParts.ContainsKey(bodyPartName))
            return null;

        // cannot detach root until all other parts are gone.

        // store in a variable
        BodyPart tmpPart = bodyParts[bodyPartName];

        // dont remove root (this).
        //if (tmpPart == GetComponent<BodyPart>())
        //    return null;
        if (tmpPart.name == "root")
            return null;

        // can only remove parts where part has no children
        //if (tmpPart.transform.childCount != 0)
        //   return null;

        bodyPartOrigins[tmpPart.name] = tmpPart.transform.localPosition;

        // set as active in the world.
        tmpPart.SetActive();

        // unparent
        tmpPart.transform.parent = null;

        // update body part list
        // TO-DO: this body parts should instead be a tree or linked list or something so
        // we dont have to keep asking for children. instead, we can just remove the root
        // of the tree!
        // remove the body part from this characters bodypart list
        // VVV VERY SLOW VVV
        //bodyParts = new List<BodyPart>(transform.GetComponentsInChildren<BodyPart>());
        bodyParts = transform.GetComponentsInChildren<BodyPart>().ToDictionary(x => x.name, x => x.GetComponent<BodyPart>());

        // check to see if both legs are now detached
        // TODO: make this not terrible... more dynamic
        // perhaps a rule class or struct with events?
        // adjust capsule collider pivot to new center and height
        //RecalculateCollisionBounds();
        

        return tmpPart;
    }

    /// <summary>
    /// Recalculates the capsule colliders bounds.
    /// </summary>
    private void RecalculateCollisionBounds()
    {
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // RECURSIVELY GO THROUGH ALL CHILDREN AND ENCAPSULATE
        Bounds initialBounds = bodyParts["root"].GetComponent<MeshFilter>().mesh.bounds;
        initialBounds.center = transform.position;
        RecalculateCollisionBounds(ref initialBounds, bodyParts["root"].gameObject);


        //initialBounds.center = initialBounds.center - transform.position;
        Debug.Log("The local bounds of this model is " + initialBounds);
        ((BoxCollider)collider).center = initialBounds.center;
        ((BoxCollider)collider).size = initialBounds.size;
        transform.rotation = currentRotation;
    }

    private void RecalculateCollisionBounds(ref Bounds currentBounds, GameObject currentBodyPart)
    {
        //Debug.Log(currentBounds.GetHashCode());
        if (currentBodyPart.transform.childCount != 0)
        {
            foreach (Transform child in currentBodyPart.transform) 
                RecalculateCollisionBounds(ref currentBounds, child.gameObject);
        }       

        //Debug.Log(string.Format("root --> {0}", currentBodyPart.name));
        //currentBounds.Encapsulate(currentBodyPart.GetComponent<Renderer>().bounds.extents);
        Bounds newBounds = currentBodyPart.GetComponent<MeshFilter>().mesh.bounds;

        //Debug.Log(newBounds.center);
        // GET CENTER OF MESH
        // DIV BY LOCAL SCALE TO MOVE CENTER TO APPROPRIATE POSITION...
        // !!!!!! THIS IS WHY EVERYTHING SHOULD IMPORT AT 1 1 1 SCALE !!!!!!!
        // RENDERER CENTER BOUNDS ARE LOCAL
        newBounds.center = (currentBodyPart.GetComponent<Renderer>().bounds.center - transform.position) / transform.localScale.x;

        //Debug.Log(newBounds.center);
        currentBounds.Encapsulate(newBounds);
        //Debug.Log(string.Format("{0} : {1} w {2}", currentBodyPart.name, currentBounds, currentBodyPart.transform.childCount));
    }

}