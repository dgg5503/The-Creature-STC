/*
    Abstract Character Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - Attach this script to the root of the character meaning its children
          contain body parts.
*/

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Inventory))]
public abstract class Character : MonoBehaviour
{
    // Body stuff
    protected List<BodyPart> bodyParts;
    protected List<RegularItem> regularItems;
    protected Dictionary<string, Vector3> bodyPartOrigins; // name_of_part : position

    // Inventory
    protected Inventory inventory;

    // Stats
    protected int defense;

    // Movement n' Physics
    protected float accelerationScalar;
    protected float rotationAccelFactor;
    protected float maxVelocity;
    protected Vector3 acceleration;
    private Vector3 velocity;
    protected new Rigidbody rigidbody;

    // States n' Actions
    protected bool isAlive;

    // actions go here...

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected virtual void Awake()
    {
        // Grab all children with BodyPart scripts and store them for reference
        // Q: Is the head/torso a body part with inf health?
        bodyParts = new List<BodyPart>(transform.GetComponentsInChildren<BodyPart>());

        // remove root from list for now
        //bodyParts.Remove(GetComponent<BodyPart>());

        // init items in hand
        regularItems = new List<RegularItem>();

        // save all INITIAL body part locallocations

        bodyPartOrigins = new Dictionary<string, Vector3>();
        foreach(BodyPart bodyPart in bodyParts)
            bodyPartOrigins.Add(bodyPart.name, bodyPart.transform.localPosition);

        // init this characters inventory
        inventory = GetComponent<Inventory>();

        // init defense modifier
        defense = 10;

        // init physics defaults
        accelerationScalar = 1.25f;
        rotationAccelFactor = .01f;
        maxVelocity = 2.0f;
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // always start alive
        isAlive = true;
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
            // TODO: When on slope, apply velocity based on norm of slope?
            velocity += acceleration * Time.deltaTime;

            // lerp directional velocity from this objects forward to the accel norm
            // creates rotation effect expected in a human.
            // looks smooth when not performing a 180
            velocity = Vector3.LerpUnclamped(transform.forward, acceleration.normalized, Time.deltaTime * rotationAccelFactor) * velocity.magnitude;

            Debug.Log(string.Format("{0}, {1} : {2}", transform.forward, acceleration.normalized, Time.deltaTime * rotationAccelFactor));
            //Debug.Log(velocity);
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
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);

        // turn the character based on veloctiy
        // dont turn when velocity is zero so we face the last direction
        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
            //transform.forward = Vector3.LerpUnclamped(transform.forward, velocity.normalized, Time.deltaTime * (accelerationScalar * 2));
        }


        // move the character based on velocity
        transform.position += (velocity * Time.deltaTime);

        // set accel to 0
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Use this function for character specific movement.
    /// i.e. player is controlled via input but AI will move based on rules.
    /// </summary>
    abstract protected void ProcessMovement();

    protected bool Attach(BodyPart bodyPartToAttach)
    {
        // place body part in first available location
        foreach (BodyPart bp in bodyParts)
        {
            if (bodyPartToAttach.SetStatic(bp, bodyPartOrigins[bodyPartToAttach.name], Quaternion.identity))
                return true;
        }
        return false;
    }

    protected BodyPart Detach(int bodyPart)
    {
        // see if part exists
        if (bodyPart >= bodyParts.Count)
            return null;

        // cannot detach root until all other parts are gone.

        // store in a variable
        BodyPart tmpPart = bodyParts[bodyPart];

        // dont remove root (this).
        if (tmpPart == GetComponent<BodyPart>())
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
        bodyParts = new List<BodyPart>(transform.GetComponentsInChildren<BodyPart>());

        // remove root again since we're getting the same list again.
        //bodyParts.Remove(GetComponent<BodyPart>());

        return tmpPart;
    }
}
