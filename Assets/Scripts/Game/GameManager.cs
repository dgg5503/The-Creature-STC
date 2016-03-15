using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Private dict for all skeletonHierarchies
    private static Dictionary<string, string> skeletonHierarchyDict = new Dictionary<string, string>();



    // Local position dictionary for each bodypart
    private static Dictionary<string, PosRot> localPosRotDict = new Dictionary<string, PosRot>();

    

    /// <summary>
    /// Gets a dictionary of all body part's initial local position.
    /// </summary>
    public static Dictionary<string, PosRot> LocalPosRotDictonary
    {
        get
        {
            return localPosRotDict;
        }
    }

    void Awake()
    { 
        // Have bparts ignore character colliders
        Physics.IgnoreLayerCollision(9, 10);

        // Build skeletal structure and grab initial positions.
        //BuildSkeletons();
        //LoadSkeletonFiles();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Builds skeletons before the game starts. Allows for picking up body parts without
    /// being attached at start.
    /// </summary>
    /*
    private void BuildSkeletons()
    {
        // Get all characters
        GameObject[] characterPrefabs = Resources.LoadAll<GameObject>("Prefabs/Characters");

        // Foreach through each character and determine:
        // BodyPart --> SubJoint
        // BodyPart rotation & postition when child of subJoint
        for (int i = 0; i < characterPrefabs.Length; ++i)
        {
            // go through all JOINTS and BODY PARTS
            Joint[] joints = characterPrefabs[i].GetComponentsInChildren<Joint>();

            // same number of joints as there are body parts.
            for (int z = 0; z < joints.Length; ++z)
                AddToJointDict(joints[z]);
        }
    }
    */

    /// <summary>
    /// Sets joint part type.
    /// Currently uses NAME of game object to define names of joints.
    /// </summary>
    /// <param name="name">Name of joint to add to dictionary.</param>
    /*
    private void AddToJointDict(Joint joint)
    {
        // if already in dict, return.
        if (jointTypeDict.ContainsValue(joint.name))
            return;

        int jointID = jointTypeDict.Count;

        // if we got here, means we didnt find our name in the dictionary, lets add it!
        jointTypeDict.Add(jointTypeDict.Count, joint.name);

        // ensure joint has atleast 1 child
        if (joint.transform.childCount < 1)
            throw new UnityException("ERROR: Joint does not have enough children!!");

        // get first child
        Transform subJoint = joint.transform.GetChild(0);

        // ensure child has only one child
        if (subJoint.childCount != 1)
            throw new UnityException("ERROR: Sub Joint does not have exactly one child!");

        // Ensure subjoint child is a body part
        BodyPart bodyPart = subJoint.GetComponentInChildren<BodyPart>();
        if (bodyPart == null)
            throw new UnityException("ERROR: Body part does not exist in sub joint!");

        // calculate joint offset proportion relative to subjoint

        // joint offset constant is simply the mag from SUBJOINT to LOCALPOS
        // which should just be mag of LOCAL POS.
        // Div this by total LENGTH of new body part

        Bounds bodyPartLocalBounds = bodyPart.GetComponent<MeshFilter>().sharedMesh.bounds;
        //Debug.Log(subJoint.TransformPoint(bodyPartLocalBounds.center));
        bodyPartLocalBounds.center = subJoint.InverseTransformPoint(bodyPart.transform.TransformPoint(bodyPartLocalBounds.center));
        Debug.Log(string.Format("{0}: {1}", bodyPart.name, bodyPart.transform.localPosition));
        Debug.Log(bodyPartLocalBounds);

        // store it
        localPosRotDict[bodyPart.name] =
            new PosRot(bodyPart.transform.localPosition, bodyPart.transform.rotation, jointID);

        // store offset proportion in subJoint

    }
    */
    /*
    /// <summary>
    /// Loads skeleton files for all model types.
    /// </summary>
    private void LoadSkeletonFiles()
    {
        // Get all txt files in ./Resources/Skeleton
        TextAsset[] skeletons = Resources.LoadAll<TextAsset>("Skeletons");

        // For loop through each text asset and load into a dictionary.
        string[] skeletonFile;
        string[] skeletonLine;
        foreach (TextAsset textAsset in skeletons)
        {
            // init dictionary
            //skeletonHierarchyDict[textAsset.name] = new Dictionary<string, string>();

            // a skeleton file consist of every line of a skeleton file
            skeletonFile = textAsset.text.Trim().Split('\n'); 

            // for each line, add relationship to dictionary
            foreach(string skeletonRelation in skeletonFile)
            {
                // split by tab
                skeletonLine = skeletonRelation.Split('\t');

                // ensure line is size 2
                if (skeletonLine.Length != 2)
                    throw new UnityException("ERROR: Skeleton file incorrectly formatted!!!");

                // setup hierarchy relations
                skeletonHierarchyDict[skeletonLine[0]] = skeletonLine[1];
            }
        }

        Debug.Log(skeletonHierarchyDict);
    }*/
}

//[InitializeOnLoad]
public static class BodyPartPreProcessing
{
    // Joint name dicitionary
    // Will be expanded if new joints are added
    private static Dictionary<int, string> jointTypeDict = new Dictionary<int, string>();

    /// <summary>
    /// Gets a dictionary of all joint types in the game (int, string).
    /// </summary>
    public static Dictionary<int, string> JointTypeDictionary
    {
        get
        {
            return jointTypeDict;
        }
    }

    static BodyPartPreProcessing()
    {
        
    }
}