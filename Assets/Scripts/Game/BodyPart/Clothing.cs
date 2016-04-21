using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Clothing requires cloth and a mesh
/// </summary>
[RequireComponent(typeof(Cloth))]
[RequireComponent(typeof(Mesh))]
public class Clothing : MonoBehaviour {

    // Fields
    /// <summary>
    /// Body part types to add for this clothing.
    /// </summary>
    [SerializeField]
    private const List<int> bodyPartColliderBindings = null;
    private List<CapsuleCollider> capsuleColliders;
    private Cloth cloth;
    // - set character
    // - parent it
    // - look at desired joints, if there is a bpart, add collider
    
    void Awake()
    {
        cloth = GetComponent<Cloth>();

        // Can only have up to the same number of expected bpart bindings.
        capsuleColliders = new List<CapsuleCollider>(bodyPartColliderBindings.Count);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // put on new clothing all together...
    public void SetCapsuleColliders(CapsuleCollider[] capsuleColliders)
    {
        // length check
        if(capsuleColliders.Length > bodyPartColliderBindings.Count)
        {
            Debug.LogError("ERROR: Number of provided capsule colliders is larger than the expected number of body parts!");
            return;
        }

        // set it
        cloth.capsuleColliders = capsuleColliders;
    }

    public bool RemoveCollider(CapsuleCollider capsuleCollider)
    {
        // find and set to null.
        int colliderArrayLen = cloth.capsuleColliders.Length;
        for (int i = 0; i < colliderArrayLen; ++i)
            if (cloth.capsuleColliders[i] == capsuleCollider) // not sure if this will work...
            {
                cloth.capsuleColliders[i] = null;
                return true;
            }

        // didnt find
        return false;
    }

    public bool AddBodyPart(BodyPart bodyPart)
    {
        // make sure body part type is in the desired list of things.
        // bodyPartColliderBindings.
        return false;

    }

    public bool AddCollider(CapsuleCollider capsuleCollider)
    {
        // find empty spot and set
        int colliderArrayLen = cloth.capsuleColliders.Length;
        for (int i = 0; i < colliderArrayLen; ++i)
            if (cloth.capsuleColliders[i] == null) // not sure if this will work...
            {
                cloth.capsuleColliders[i] = capsuleCollider;
                return true;
            }

        // didnt find
        return false;
    }
}
