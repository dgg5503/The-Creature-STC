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

public class Joint : MonoBehaviour {
    // Joint information
    private Joint parentJoint; // every joint can only have ONE PARENT but can have MULTIPLE CHILDREN

    // Proportional offset value based off of initial body part connected.
    // Should allow any other body part to connect and offset from this joint similarly
    // to the original body part.
    // Formula used will be:
    // offset = thisJointLocalPos + (DirectionOfInitialRotation ( modelLengthOnAxisOfDirectionOfInitRot * PreCalcOffsetConst) )
    // PreCalcOffsetConst = 
    [SerializeField]
    float offsetProportion = 0; // calculated at game start once.

    [SerializeField]
    private int jointType = -1; // joint type (this should never change)


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
    public Joint Parent
    {
        get
        {
            return parentJoint;
        }
    }

    /// <summary>
    /// Get the offset proportion for this specific joint that will be used for given body parts attached to this joint.
    /// </summary>
    public float OffsetProportion
    {
        get
        {
            return offsetProportion;
        }

        set
        {
            offsetProportion = value;
        }
    }

    /// <summary>
    /// Get the single bodypart currently connected to this joint.
    /// </summary>
    public BodyPart BodyPart
    {
        get
        {
            return GetComponentInChildren<BodyPart>();
        }
    }

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    void Awake()
    {
        // add this joint part type to our list of joint types
        //Initialize();

        // init parent joint as null
        parentJoint = null;

        // Get parent joint (if there is one)
        Joint[] joints = GetComponentsInParent<Joint>();
        if (joints.Length > 1)
            parentJoint = joints[1];

        // Get first occurence of subjoint (IF ONE DOESNT EXIST, THROW ERROR)
        //if ((subJoint = GetComponentInChildren<SubJoint>()) == null)
        //    throw new UnityException(string.Format("ERROR: NO SUBJOINT FOUND FOR {0}.", name));

        // set joint reference in body part
        //subJoint.BodyPart.Joint = this;

        // set tag
        tag = "Joint";
    }

    // Use this for initialization
    void Start() {
        if(parentJoint != null)
            Debug.Log(string.Format("{0} --> {1}", parentJoint.name, name));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
