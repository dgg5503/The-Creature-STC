using UnityEngine;
using System.Collections.Generic;

public class Skeleton : MonoBehaviour {
    // Holds initial valid names of body parts

    // List of body parts
    protected List<BodyPart> bodyParts;



    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    void Awake()
    {
        // Grab all children with BodyPart scripts and store them for reference
        // Q: Is the head/torso a body part with inf health?
        bodyParts = new List<BodyPart>(transform.GetComponentsInChildren<BodyPart>());
    }
    

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Detaches from the currently attached to parent. Will promptly turn into
    /// a physics object and fall to the ground!
    /// </summary>
    /// <returns>This body p</returns>
    void Detach(BodyPart bodyPart)
    {
        // see if there is a parent
        if (bodyParts.IndexOf(bodyPart) != -1)
            return;

        // unparent
        bodyPart.transform.parent = null;

        // turn off kinematics
        bodyPart.Detach();
    }
}
