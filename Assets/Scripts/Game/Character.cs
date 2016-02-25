/*
    Abstract Character Class
    ------------------------
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

    // Inventory
    protected Inventory inventory;

    // Stats
    protected int defense;

    // Movement n' Physics
    protected Vector3 acceleration;
    protected Vector3 velocity;

    // States n' Actions
    protected bool isAlive;
    // actions go here...

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    void Awake()
    {
        // Grab all children with BodyPart scripts and store them for reference
        // Q: Is the head/torso a body part with inf health?
        bodyParts = new List<BodyPart>(transform.GetComponentsInChildren<BodyPart>());

        // remove root from list for now
        bodyParts.Remove(GetComponent<BodyPart>());

        regularItems = new List<RegularItem>();

        inventory = GetComponent<Inventory>();

        defense = 10;

        acceleration = Vector3.zero;
        velocity = Vector3.zero;

        isAlive = true;
    }

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    protected BodyPart Detach(int bodyPart)
    {
        // see if part exists
        if (bodyPart >= bodyParts.Count)
            return null;

        // cannot detach root until all other parts are gone.

        // store in a variable
        BodyPart tmpPart = bodyParts[bodyPart];

        // can only remove parts where part has no children
        //if (tmpPart.transform.childCount != 0)
        //   return null;

        if (tmpPart.transform.childCount > 2)
            return null;

        // unparent
        tmpPart.transform.parent = null;

        // turn off kinematics
        tmpPart.SetActive();

        // update body part list
        // TO-DO: this body parts should instead be a tree or linked list or something so
        // we dont have to keep asking for children. instead, we can just remove the root
        // of the tree!
        // remove the body part from this characters bodypart list
        // VVV VERY SLOW VVV
        bodyParts = new List<BodyPart>(transform.GetComponentsInChildren<BodyPart>());

        // remove root from list for now
        bodyParts.Remove(GetComponent<BodyPart>());

        return tmpPart;
    }
}
