﻿using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;

/*
    TODO:
    - Add more checks for adding specific new characters
    - Perhaps make a copy character first then create links
    - Load in skeleton info with unity (shouldnt serialize do this???)

    - move root GO to torso center
    - move all joints to child of TORSO
*/

class GenerateBodyPartPrefabs : EditorWindow {
    private static Dictionary<int, string> jointTypeDict = new Dictionary<int, string>();
    private Dictionary<string, GameObject> createdPrefabs;

    [MenuItem("Body Parts/Rebuild Body Part Prefabs")]
    static void Init()
    {

        // load data into dictionary when null
        TextAsset textAsset = Resources.Load<TextAsset>("joints");

        if (textAsset != null)
        {
            // split at \n and \t
            string[] lines = textAsset.text.Split('\n');
            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i] != "")
                {
                    string[] data = lines[i].Split('\t');

                    jointTypeDict[int.Parse(data[0])] = data[1];
                }
            }
        }


        //EditorWindow window = GetWindow(typeof(GenerateBodyPartPrefabs), false, "Body Part Prefab Generator");
        GetWindow(typeof(GenerateBodyPartPrefabs), false, "Body Part Prefab Generator");
    }

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
            createdPrefabs = Resources.LoadAll<GameObject>("Prefabs/").ToDictionary(x => x.name, x => x);
            GameObject[] selectedObjects = Selection.gameObjects;
            //Dictionary<string, List<GameObject>> currentConnections = BuildPrefabConnections(); // needed??
            foreach (GameObject go in selectedObjects)
                AddCharacter(go);
            ReconnectAllPrefabs(BuildPrefabConnections(), true);
        }

        // Rebuild all
        /*
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
        */
    }
    
    private void AddCharacter(GameObject character)
    {
        if(character == null || character.activeInHierarchy)
        {
            Debug.Log("ERROR: Provided character or all game objs is NULL or character is present in the hierarchy.");
            return;
        }

        // make sure we didnt already create the character
        if (createdPrefabs.ContainsKey(character.name))
        {
            Debug.Log("ERROR: prefab already created");
            return;
        }

        // final character name
        string characterName = character.name;

        // create tmp character
        character = Instantiate(character);

        // hold root bodypart
        BodyPart rootBodyPart = null;
        CustomJoint rootJoint = null;

        // APPLY CHANGES
        // Go through all joints and process relationships
        CustomJoint[] joints = character.GetComponentsInChildren<CustomJoint>();
        List<BodyPart> leafBodyParts = new List<BodyPart>(); // change to array?
        List<BodyPart> allBodyParts = new List<BodyPart>();
        // verify joint layout
        for (int z = 0; z < joints.Length; ++z)
        {
            // add to joint dict
            AddToJointDict(joints[z]);

            // ensure joint has atleast 1 child
            if (joints[z].transform.childCount < 1)
                throw new UnityException("ERROR: Joint does not have enough children!!");

            // Ensure subjoint child is a body part
            BodyPart bodyPart = null;
            foreach (Transform child in joints[z].transform)
                if ((bodyPart = child.GetComponent<BodyPart>()) != null)
                {
                    allBodyParts.Add(bodyPart);
                    break;
                }

            if (bodyPart == null)
                throw new UnityException("ERROR: Body part does not exist in joint: " + joints[z].name);

            bodyPart.BodyPartType = joints[z].JointType;
            //bodyPart.InitialLocalPosition = bodyPart.transform.localPosition;
            //bodyPart.InitialLocalRotation = bodyPart.transform.localRotation;

            if (bodyPart.name.ToLower().Contains("torso"))
            {
                rootJoint = joints[z];
                rootBodyPart = bodyPart;
            }
            else if (joints[z].GetComponentsInChildren<CustomJoint>().Length == 1)
            {
                Debug.Log(bodyPart.name + " has no child ");
                leafBodyParts.Add(bodyPart);
            }

            

            // connect prefabs if they exist in scene
            /*
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
                
                // remove all null go's
                gameObjPrefab[bodyPart.name].RemoveAll(x => x == null);

                // remove all 0 count lists
                if (gameObjPrefab[bodyPart.name].Count == 0)
                    gameObjPrefab.Remove(bodyPart.name);
            }
            */
        }

        // construct tmp character with following layout
        // BASE NAME (delete animator)
        // - bodyParts 
        // find torso bodypart and set as root.
        rootBodyPart.transform.parent = character.transform;

        //GameObject bodyPartRoot = character.transform.f
        //bodyPartRoot.transform.parent = character.transform;
        //bodyPartRoot.transform.localPosition = Vector3.zero;
        //bodyPartRoot.transform.localRotation = Quaternion.identity;

        //      -- bparts --
        for (int i = 0; i < leafBodyParts.Count; ++i)
            IsolateBodyPart(leafBodyParts[i], rootBodyPart);
        rootBodyPart.transform.localPosition = Vector3.zero;
        // - skeleton (animator)
        //GameObject skeletonRoot = new GameObject("skeleton");
        rootJoint.transform.parent = character.transform;
        rootJoint.transform.localPosition = Vector3.zero;
        rootJoint.transform.localRotation = Quaternion.identity;

        // copy animator
        Animator animator;
        if ((animator = character.GetComponent<Animator>()) != null)
            if (UnityEditorInternal.ComponentUtility.CopyComponent(animator))
                if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(rootJoint.gameObject))
                {
                    DestroyImmediate(animator);
                    Debug.Log("Copied animator as well...");
                }

        //      -- joints --
        //      set immediate joints to skeleton gameobject.
        CustomJoint[] childJoints = character.GetComponentsInChildren<CustomJoint>();
        for(int i = 0; i < childJoints.Length; ++i)
            if(childJoints[i].transform.parent == character.transform)
                childJoints[i].transform.parent = rootJoint.transform;

        // CREATE PREFABS
        // generate prefabs
        for(int i = 0; i < allBodyParts.Count; ++i)
            if (!createdPrefabs.ContainsKey(allBodyParts[i].name))
                createdPrefabs[allBodyParts[i].name] = PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/BodyParts/" + allBodyParts[i].name + ".prefab", allBodyParts[i].gameObject, ReplacePrefabOptions.ConnectToPrefab);

        createdPrefabs[characterName] = PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/Characters/" + characterName + ".prefab", character, ReplacePrefabOptions.ConnectToPrefab);

        // CONNECT OBJS TO PREFABS AT THEIR ROOT


        // connect prefabs if they exist in scene
        /*
        if (gameObjPrefab.ContainsKey(character.name))
        {
            for (int h = 0; h < gameObjPrefab[character.name].Count; ++h)
            {
                if (gameObjPrefab[character.name][h] != null)
                {
                    // Save rotation and location, then connect
                    Vector3 savedLocation = gameObjPrefab[character.name][h].transform.position;
                    Quaternion saveRotation = gameObjPrefab[character.name][h].transform.rotation;

                    GameObject finalConnection = PrefabUtility.ConnectGameObjectToPrefab(gameObjPrefab[character.name][h], createdPrefabs[character.name]);

                    // bring back pos and rot
                    finalConnection.transform.position = savedLocation;
                    finalConnection.transform.rotation = saveRotation;
                }
            }

            // remove all null go's
            gameObjPrefab[character.name].RemoveAll(x => x == null);

            // remove all 0 count lists
            if (gameObjPrefab[character.name].Count == 0)
                gameObjPrefab.Remove(character.name);
        }

        */
        DestroyImmediate(character);
    }

    private void ReconnectAllPrefabs(Dictionary<string, List<GameObject>> gameObjPrefab, bool revert)
    {
        if (gameObjPrefab == null || createdPrefabs == null)
        {
            Debug.Log("ERROR: Provided dictionary or created prefabs is NULL.");
            return;
        }

        foreach(KeyValuePair<string, GameObject> kvp in createdPrefabs)
        {
            if(gameObjPrefab.ContainsKey(kvp.Key))
            {
                // hilolf
                // connect all game objs to prefab
                for(int i = 0; i < gameObjPrefab[kvp.Key].Count; ++i)
                {
                    // Could be faster if  had all parents before hand.
                    GameObject rootGO = PrefabUtility.FindRootGameObjectWithSameParentPrefab(gameObjPrefab[kvp.Key][i]);

                    if (rootGO == gameObjPrefab[kvp.Key][i])
                    {
                        // Save rotation and location, then connect
                        Vector3 savedLocation = gameObjPrefab[kvp.Key][i].transform.position;
                        Quaternion saveRotation = gameObjPrefab[kvp.Key][i].transform.rotation;

                        GameObject go = PrefabUtility.ConnectGameObjectToPrefab(gameObjPrefab[kvp.Key][i], kvp.Value);
                        if (go && revert)
                            PrefabUtility.RevertPrefabInstance(go);

                        // bring back pos and rot
                        go.transform.position = savedLocation;
                        go.transform.rotation = saveRotation;
                    }
                }
            }
        }
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
            AddCharacter(characterPrefabs[i]);
        }

        ReconnectAllPrefabs(BuildPrefabConnections(), true);
    }

    private void AddToJointDict(CustomJoint joint)
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

    void IsolateBodyPart(BodyPart leafBodyPart, BodyPart bodyPartRef)
    {
        // Grab the parent joint relative to this bpart
        CustomJoint[] parentJoint;
        
        if ((parentJoint = leafBodyPart.transform.GetComponentsInParent<CustomJoint>()) == null ||
            parentJoint.Length <= 1)
        {
            leafBodyPart.transform.parent = bodyPartRef.transform;
            bodyPartRef.endPoints[leafBodyPart.BodyPartType] = leafBodyPart.transform.localPosition;
            return;
        }

        // look for first bodypart in the joint since every joint should
        // only have 1 body part.
        BodyPart parentBodyPart = null;
        foreach (Transform child in parentJoint[1].transform)
        {
            if ((parentBodyPart = child.GetComponent<BodyPart>()) != null)
            {
                // set leaf to parent and call func again from parent
                leafBodyPart.transform.parent = parentBodyPart.transform;
                parentBodyPart.endPoints[leafBodyPart.BodyPartType] = leafBodyPart.transform.localPosition;
                IsolateBodyPart(parentBodyPart, bodyPartRef);
                break;
            }
        }
    }

    void OnDestroy()
    {
        // save dictionary to text asset
        string finalFile = "";
        foreach (KeyValuePair<int, string> kvp in jointTypeDict)
            finalFile += kvp.Key + "\t" + kvp.Value + "\n";

        File.WriteAllText(Application.dataPath + "/Resources/joints.txt", finalFile);
        AssetDatabase.SaveAssets();
        
    }
}
