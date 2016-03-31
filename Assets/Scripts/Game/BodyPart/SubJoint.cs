/*
    SubJoit Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
        
    DONE

    NOTES
        - Holds references to the SINGLE body part associated with this subjoint.
*/

using UnityEngine;
using System.Collections;

public class SubJoint : MonoBehaviour {

    // !! SINGLE !! body part associated with this subjoint
    private CustomJoint joint; // parent
    private BodyPart bodyPart; // child

    // Proportional offset value based off of initial body part connected.
    // Should allow any other body part to connect and offset from this joint similarly
    // to the original body part.
    // Formula used will be:
    // offset = thisJointLocalPos + (DirectionOfInitialRotation ( modelLengthOnAxisOfDirectionOfInitRot * PreCalcOffsetConst) )
    // PreCalcOffsetConst = 
    float offsetProportion; // calculated at game start once.


    /// <summary>
    /// Returns the parent joint to this subjoint (this should never change).
    /// </summary>
    public CustomJoint Joint
    {
        get
        {
            return joint;
        }
    }

    /// <summary>
    /// Get the single body part that is currently connected to this subjoint.
    /// </summary>
    public BodyPart BodyPart
    {
        get
        {
            return GetComponentInChildren<BodyPart>();
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
    }

    void Awake()
    {
        // force tag as subjoint
        tag = "subJoint";

        // grab parent joint reference, if none, throw error.
        if ((joint = GetComponentInParent<CustomJoint>()) == null)
            throw new UnityException(string.Format("ERROR: Joint not found for subjoint {0}.", name));

        // grab body part reference, if none, throw error.
        if ((bodyPart = GetComponentInChildren<BodyPart>()) == null)
            throw new UnityException(string.Format("ERROR: Bodypart not found for subjoint {0}.", name));
    }

	// Use this for initialization
	void Start () {
        // Set offset proportion at start 
       // offsetProportion = GameManager.LocalPosRotDictonary[bodyPart.name].position.magnitude;
        //Debug.Log(offsetProportion);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
