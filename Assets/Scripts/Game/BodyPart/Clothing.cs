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

    /// <summary>
    /// Gets the expected body part colliders for this clothing.
    /// </summary>
    public int[] ExpectedBodyPartColliders { get { return expectedBodyPartColliders; } }

    public Cloth Cloth
    {
        get
        {
            if(cloth == null)
                cloth = GetComponent<Cloth>();
            return cloth;
        }
    }

    // - set character
    // - parent it
    // - look at desired joints, if there is a bpart, add collider
    
    void Awake()
    {
        cloth = GetComponent<Cloth>();
        colliders = new List<CapsuleCollider>(cloth.capsuleColliders);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnBecameInvisible()
    {
        //cloth.enabled = false;
    }

    void OnBecameVisible()
    {
        //cloth.enabled = true;
    }

    public bool AddBodyPart(BodyPart bodyPart)
    {
        // OPTIMIZE: use list???
        CapsuleCollider colliderToAdd = bodyPart.GetComponent<CapsuleCollider>();
        if (expectedBodyPartColliders.Contains(bodyPart.BodyPartType) &&
            !colliders.Contains(colliderToAdd))
        {
            colliders.Add(colliderToAdd);
            cloth.capsuleColliders = colliders.ToArray();
            
            return true;
        }
        return true;
    }

    public bool RemoveBodyPart(BodyPart bodyPart)
    {
        CapsuleCollider colliderToRemove = bodyPart.GetComponent<CapsuleCollider>();
        if(expectedBodyPartColliders.Contains(bodyPart.BodyPartType) &&
            colliders.Contains(colliderToRemove))
        {
            colliders.Remove(colliderToRemove);
            cloth.capsuleColliders = colliders.ToArray();
            return true;
        }
        
        return false;
    }
}
