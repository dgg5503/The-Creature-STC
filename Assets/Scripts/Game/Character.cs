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
    protected float maxVelocity;
    protected Vector3 acceleration;
    protected Vector3 velocity;
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
        maxVelocity = 2.0f;
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();

        // always start alive
        isAlive = true;
    }

	// Use this for initialization
	void Start ()
    {
	    
	}

    // Update is called once per frame
    protected virtual void Update ()
    {
        //bodyParts[2].Help();
        velocity += acceleration;
        Vector3.ClampMagnitude(velocity, maxVelocity);
        //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.position += (velocity * Time.deltaTime);
        //rigidbody.AddForce(acceleration, ForceMode.VelocityChange);


    }

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
