/*
    Joint Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
        
    DONE

    - FIGURE OUT WHAT BODY PARTS GO WITH WHAT BODY PARTS SO WE CAN SPAWN w/o BPARTS


    NOTES
        - Holds references to the SINGLE sub joint associated with this joint.
*/

using UnityEngine;
using System.Collections.Generic;

public class CustomJoint : MonoBehaviour {
    // Joint information
    private CustomJoint parentJoint; // this should be set once and then never change!

    [SerializeField]
    private int jointType = -1; // joint type (this should never change)

    BodyPart attachedBodyPart;


    /// <summary>
    /// Gets the joint type of this joint as an integer. DO NOT SET THIS VALUE !!EVER!!
    /// </summary>
    public int JointType
    {
        get
        {
            return jointType;
        }

        set
        {
            jointType = value;
        }
    }

    /// <summary>
    /// Gets this joints parent.
    /// </summary>
    public CustomJoint Parent
    {
        get
        {
            //Debug.Log("The parent is " + parentJoint.name);
            //return transform.parent.GetComponent<CustomJoint>();
            return parentJoint;
        }
    }

    /// <summary>
    /// Get the single bodypart currently connected to this joint.
    /// </summary>
    public BodyPart BodyPart
    {
        get
        {
            return attachedBodyPart;
        }

        set
        {
            if (value == null || value.BodyPartType == jointType)
                attachedBodyPart = value;
            else
                Debug.Log("ERROR: Trying to attach " + value.name +
                    " to " + jointType + " but its " + value.BodyPartType);
        }
    }
    

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    void Awake()
    {
        parentJoint = transform.parent.GetComponent<CustomJoint>();
        tag = "Joint";
    }

    // Use this for initialization
    void Start() {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
