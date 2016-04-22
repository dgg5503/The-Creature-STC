using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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
    private int[] expectedBodyPartColliders = null;
    private List<CapsuleCollider> colliders;
    private Cloth cloth;

    // - set character
    // - parent it
    // - look at desired joints, if there is a bpart, add collider
    
    void Awake()
    {
        cloth = GetComponent<Cloth>();
        colliders = new List<CapsuleCollider>(cloth.capsuleColliders);

        // init array of capsule colliders.
        // neeeded???
        //cloth.capsuleColliders = new CapsuleCollider[expectedBodyPartColliders.Length];
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool AddBodyPart(BodyPart bodyPart)
    {
        // OPTIMIZE: use list???
        CapsuleCollider colliderToAdd = bodyPart.GetComponent<CapsuleCollider>();
        if (!colliders.Contains(colliderToAdd))
        {
            colliders.Add(colliderToAdd);
            /*
            int storeIndex = -1;
            for (int i = 0; i < expectedBodyPartColliders.Length; ++i)
                if (expectedBodyPartColliders[i] == bodyPart.BodyPartType) // look for space in cloth array, also ensure that same collider isnt in there
                    for (int z = 0; z < cloth.capsuleColliders.Length; ++z)
                        if (cloth.capsuleColliders[z] == null)
                            storeIndex = z;
                        else if (cloth.capsuleColliders[z] == colliderToAdd)
                            return false;

            // set collider
            if (storeIndex == -1)
                return false;
                */
            cloth.capsuleColliders = colliders.ToArray();
            return true;
        }
        return true;
    }

    public bool RemoveBodyPart(BodyPart bodyPart)
    {
        CapsuleCollider colliderToRemove = bodyPart.GetComponent<CapsuleCollider>();
        if(colliders.Contains(colliderToRemove))
        {
            colliders.Remove(colliderToRemove);

            /*
            for (int i = 0; i < cloth.capsuleColliders.Length; ++i)
                if (cloth.capsuleColliders[i] == colliderToRemove)
                {
                    cloth.capsuleColliders[i] = null;
                    return true;
                }
            */

            cloth.capsuleColliders = colliders.ToArray();
            return true;
        }
        
        return false;
    }
}
