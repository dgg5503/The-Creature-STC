using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FreezeScale : EditorWindow {

    //private static List<Mesh> lastModifiedGameObjects;


    [MenuItem("GameObject/Freeze Scale")]
    static void Init()
    {
        // Spawn window
        EditorWindow window = GetWindow(typeof(FreezeScale), false, "Freeze Scale");
        window.minSize = new Vector2(305, 105);
        window.maxSize = new Vector2(305, 105);
        
        //Undo.undoRedoPerformed = new Undo.UndoRedoCallback(RecalculateOldBounds);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Freeze Currently Selected Objects",
            GUILayout.Width(300),
            GUILayout.Height(100)))
        {
            // grab currently selected go's
            GameObject[] gameObjects = Selection.gameObjects;
            MeshFilter meshFilter;
            Mesh sharedMesh;
            Vector3 currScale;

            Undo.SetCurrentGroupName("Modified Mesh Group");
            int group = Undo.GetCurrentGroup();

            // foreach all of them
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                // ensure a mesh filter exists
                if ((meshFilter = gameObjects[i].GetComponent<MeshFilter>()) != null &&
                    gameObjects[i].transform.lossyScale != Vector3.one)
                {
                    Undo.RecordObject(meshFilter, "Mesh Filter Modified");
                    Undo.RecordObject(meshFilter.sharedMesh, "Shared Mesh Modified");
                    Undo.RecordObject(gameObjects[i].transform, "Transform Modified");

                    
                    // grab current scale
                    currScale = gameObjects[i].transform.localScale;

                    // clone mesh
                    meshFilter.sharedMesh = (Mesh)Instantiate(meshFilter.sharedMesh);

                    // get shared mesh reference
                    sharedMesh = meshFilter.sharedMesh;

                    // grab verts
                    Vector3[] verts = sharedMesh.vertices;

                    // mult each component by relative componenet
                    for (int z = 0; z < verts.Length; ++z)
                        verts[z] = new Vector3(verts[z].x * currScale.x, verts[z].y * currScale.y, verts[z].z * currScale.z);

                    // set verts
                    sharedMesh.vertices = verts;

                    // recalc bounds
                    sharedMesh.RecalculateBounds();

                    
                    //AddToStaticList(sharedMesh);
                   
                    // set GO to 1,1,1 scale
                    gameObjects[i].transform.localScale = Vector3.one; 
                }   
            }
            Undo.CollapseUndoOperations(group);
        }
    }
    /*
    void AddToStaticList(Mesh mesh)
    {
        if (lastModifiedGameObjects == null)
            lastModifiedGameObjects = new List<Mesh>();
        lastModifiedGameObjects.Add(mesh);
    }
    */


    /*
    static void RecalculateOldBounds()
    {
        if(lastModifiedGameObjects != null)
        {
            for (int i = 0; i < lastModifiedGameObjects.Count; ++i)
            {
                lastModifiedGameObjects[i].RecalculateBounds();
                Debug.Log("Recalcd " + lastModifiedGameObjects[i].name);
            }
        }
    }
    */

}
