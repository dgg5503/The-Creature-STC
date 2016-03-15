using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

class GenerateBodyPartPrefabs : EditorWindow {

    [MenuItem("Body Parts/Rebuild Body Part Prefabs")]
    static void Init()
    {
        GenerateBodyPartPrefabs window =
            (GenerateBodyPartPrefabs)EditorWindow.GetWindow(typeof(GenerateBodyPartPrefabs), false, "Body Part Prefab Generator");
    }

    // Joint name dicitionary
    // Will be expanded if new joints are added
    [SerializeField]
    private static Dictionary<int, string> jointTypeDict = new Dictionary<int, string>();
    private Vector2 scroll;

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(200), GUILayout.Height(300));
            {
                //GameObject[] gameObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                foreach (KeyValuePair<int, string> kvp in jointTypeDict)
                {
                    EditorGUILayout.LabelField(kvp.Key + " - " + kvp.Value);
                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndHorizontal();

        // add selected
        if (GUILayout.Button("Add Currently Selected Prefabs"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            Dictionary<string, List<GameObject>> currentConnections = BuildPrefabConnections();
            foreach (GameObject go in selectedObjects)
                AddCharacter(go, currentConnections, true);
        }

        // Rebuild all
        if (GUILayout.Button("Rebuild ALL Body Part Prefabs"))
        {
            if (EditorUtility.DisplayDialog("Rebuild All Body Part Prefabs",
                "Are you sure you want to rebuild ALL body part prefabs? " +
                "This will reset all settings for existing body part prefabs!",
                "Yes",
                "No"))
            {
                GenerateBodyParts();
            }
        }
    }
    
    private void AddCharacter(GameObject character, Dictionary<string, List<GameObject>> gameObjPrefab, bool overWriteBodyPrefabs)
    {
        // Get current prefab connections for character
        // creat tmp dict of currently created prefabs
        Dictionary<string, GameObject> createdPrefabs;
        if(overWriteBodyPrefabs)
            createdPrefabs = new Dictionary<string, GameObject>();
        else
            createdPrefabs = Resources.LoadAll<GameObject>("Prefabs/BodyParts").ToDictionary(x=>x.name, x=>x);

        // Go through all joints and process relationships
        Joint[] joints = character.GetComponentsInChildren<Joint>();

        // verify joint layout
        for (int z = 0; z < joints.Length; ++z)
        {
            // add to joint dict
            AddToJointDict(joints[z]);

            //Debug.Log(joints[z].name + " : " + joints[z].JointTest);

            // ensure joint has atleast 1 child
            if (joints[z].transform.childCount < 1)
                throw new UnityException("ERROR: Joint does not have enough children!!");

            // get first child
            //Transform subJoint = joints[z].transform.GetChild(0);

            // ensure child has only one child
            //if (subJoint.childCount != 1)
            //    throw new UnityException("ERROR: Sub Joint does not have exactly one child!");

            // Ensure subjoint child is a body part
            BodyPart bodyPart = null;
            foreach (Transform child in joints[z].transform)
                if ((bodyPart = child.GetComponent<BodyPart>()) != null)
                    break;

            if (bodyPart == null)
                throw new UnityException("ERROR: Body part does not exist in joint: " + joints[z].name);

            //Debug.Log(joints[z].name + " --> " + bodyPart.name);

            // calculate joint offset proportion relative to subjoint

            // joint offset constant is simply the mag from SUBJOINT to LOCALPOS
            // which should just be mag of LOCAL POS.
            // Div this by total LENGTH of new body part

            //Bounds bodyPartLocalBounds = bodyPart.GetComponent<MeshFilter>().sharedMesh.bounds;
            //Debug.Log(subJoint.TransformPoint(bodyPartLocalBounds.center));
            //bodyPartLocalBounds.center = subJoint.InverseTransformPoint(bodyPart.transform.TransformPoint(bodyPartLocalBounds.center));
            //Debug.Log(string.Format("{0}: {1}", bodyPart.name, bodyPart.transform.localPosition));
            //Debug.Log(bodyPartLocalBounds);

            // store it
            //localPosRotDict[bodyPart.name] =
            //    new PosRot(bodyPart.transform.localPosition, bodyPart.transform.rotation, jointID);




            // CANT GRAB BY NAME SINCE WHAT IF MULTIPLE OF SAME BODY PART?
            // This is where ID TYPE will come in handy
            // ok this is how it works.
            // we do a grab by name ONCE (at regenerate) to generate body part relations
            // use body parts found in BODYPARTS folder to place body parts!
            // should replace if needed


            bodyPart.BodyPartType = joints[z].JointType;

            // update existing values based on existing parent prefabs
            // reconnect if it exists!
            /*
            if (gameObjPrefab.ContainsKey(bodyPart.name))
            {
                for (int x = 0; x < gameObjPrefab[bodyPart.name].Count; ++x)
                {
                    //var instanceRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(bodyPart.gameObject);
                    //var targetPrefab = PrefabUtility.GetPrefabParent(instanceRoot);

                    Object parentPrefab = PrefabUtility.GetPrefabParent(gameObjPrefab[bodyPart.name][x]);
                    //PrefabUtility.ConnectGameObjectToPrefab(bodyPart.gameObject, (GameObject)parentPrefab);

                    // Get modifications based on existing body part prefabs.
                    //PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(bodyPart.gameObject); // WORKS
                    PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(parentPrefab);

                    if (modifications != null)
                    {

                        Debug.Log(modifications.LongLength);
                        for (int h = 0; h < modifications.Length; ++h)
                            Debug.Log(modifications[h].propertyPath + " : " + modifications[h].value + " - " + modifications[h].target + " - " + modifications[h].objectReference.name);

                        //Debug.Log("Parent: " + parentPrefab.name);
                        //PrefabUtility.SetPropertyModifications(parentPrefab, modifications);
                        PrefabUtility.SetPropertyModifications(gameObjPrefab[bodyPart.name][x], modifications); // WORKS
                        //Debug.Log("Modified: " + gameObjPrefab[bodyPart.name][x].name);

                        //PrefabUtility.ReplacePrefab(PrefabUtility.FindRootGameObjectWithSameParentPrefab(gameObjPrefab[bodyPart.name][x]), gameObjPrefab[bodyPart.name][x], ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
                //PrefabUtility.ConnectGameObjectToPrefab(gameObjPrefab[newPrefab.name][x], newPrefab);
            }
            else
            {
                // set joint relationships in joints
                // recreate prefabs to break connections
                // seb body part 
            }
            */

            // generate prefabs
            if (!createdPrefabs.ContainsKey(bodyPart.name))
            {
                createdPrefabs[bodyPart.name] = PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/BodyParts/" + bodyPart.name + ".prefab", bodyPart.gameObject, ReplacePrefabOptions.ConnectToPrefab);
            } 

            // connect prefabs if they exist in scene
            if (gameObjPrefab.ContainsKey(bodyPart.name))
            {
                for (int h = 0; h < gameObjPrefab[bodyPart.name].Count; ++h)
                {
                    if (gameObjPrefab[bodyPart.name][h] != null)
                    {
                        // Save rotation and location, then connect
                        Vector3 savedLocation = gameObjPrefab[bodyPart.name][h].transform.position;
                        Quaternion saveRotation = gameObjPrefab[bodyPart.name][h].transform.rotation;

                        

                        GameObject finalConnection = PrefabUtility.ConnectGameObjectToPrefab(gameObjPrefab[bodyPart.name][h], createdPrefabs[bodyPart.name]);

                        // bring back pos and rot
                        finalConnection.transform.position = savedLocation;
                        finalConnection.transform.rotation = saveRotation;
                    }
                }

                //gameObjPrefab[newPrefab.name].RemoveAll(null);
            }
        }

        // Reconnect root back to prefab (if any?)
        if (gameObjPrefab.ContainsKey(character.name))
        {
            for (int h = 0; h < gameObjPrefab[character.name].Count; ++h)
            {
                // Reconnect
                if (PrefabUtility.ReconnectToLastPrefab(gameObjPrefab[character.name][h]))
                {
                    // revert only joint scripts, grab all joints for this character
                    Joint[] charJoints = gameObjPrefab[character.name][h].GetComponentsInChildren<Joint>();
                    for (int z = 0; z < charJoints.Length; ++z)
                    {
                        if (!PrefabUtility.ResetToPrefabState(charJoints[z]))
                        {
                            Debug.Log("ERROR REVERTING JOINTS");
                        }
                    }
                }
                else
                {
                    Debug.Log("ERROR RECONNECTING");
                }
            }
        }

        // add prefabs if needed.
    }

    private Dictionary<string, List<GameObject>> BuildPrefabConnections()
    {
        // Get all current game objects
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();

        // create tmp dict which will hold current connections between game objects and prefabs
        // name of prefab, gameobject its connected to.
        Dictionary<string, List<GameObject>> gameObjPrefab = new Dictionary<string, List<GameObject>>();

        // build dict of prefab connections
        Object prefab;
        for (int i = 0; i < allGameObjects.Length; ++i)
        {
            if ((prefab = PrefabUtility.GetPrefabParent(allGameObjects[i])) != null)
            {
                if (!gameObjPrefab.ContainsKey(prefab.name))
                    gameObjPrefab[prefab.name] = new List<GameObject>();
                gameObjPrefab[prefab.name].Add(allGameObjects[i]);
                //Debug.Log("GO: " + allGameObjects[i].name + " --> " + prefab.name);
            }
        }

        return gameObjPrefab;
    }

    private void GenerateBodyParts()
    {
        // clear joint dict
        jointTypeDict.Clear();

        // load all characters in their built form for skeleton processing.
        GameObject[] characterPrefabs = Resources.LoadAll<GameObject>("Prefabs/Characters");

        // go through each character and set up ALL body part / subjoint / joint related info.
        for (int i = 0; i < characterPrefabs.Length; ++i)
        {
            AddCharacter(characterPrefabs[i], BuildPrefabConnections(), true);
        }
    }

    private void AddToJointDict(Joint joint)
    {
        // Go through dict and try to find matching string name.
        foreach (KeyValuePair<int, string> jointInfo in jointTypeDict)
        {
            // if found, set and break out of loop
            if (jointInfo.Value == joint.name)
            {
                joint.JointType = jointInfo.Key;
                return;
            }
        }

        //Debug.Log(joints[z].name + " : " + joints[z].JointTest);

        // otherwise add to dict
        int jointID = jointTypeDict.Count;
        jointTypeDict.Add(jointTypeDict.Count, joint.name);

        // set joint type for joint
        joint.JointType = jointID;
    }
}
