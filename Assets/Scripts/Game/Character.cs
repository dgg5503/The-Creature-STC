/*
    Abstract Character Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
        + CUT COLLIDER IN HALF WHEN NO LEGS
        + IGNORE CAPSULE COLLIDER WHEN DETACHING
        + MAKE DEAD GUY KINEMATIC

        + PICK UP AND CARRY B PARTS (HIT E)
        + CLICK ON BODY PART TO DETACH (FOR PLAYER)
        + CLICK ON BODY PART OF DEAD TO DETACH (when close)

        - is grounded
    NOTES
        - Attach this script to the root of the character meaning its children
          contain body parts.
*/

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public enum CharacterState
{
    None,
    Idle,
    Walk,
    Fall
}

public enum GrappleState
{
    None,
    Grapple_Right,
    Grapple_Left,
    Fly_Right,
    Fly_Left
}

public enum ThrowState
{
    None,
    Throw_Left,
    Throw_Right
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Inventory))]
public abstract class Character : MonoBehaviour
{
    // Body stuff
    protected Dictionary<int, CustomJoint> joints;
    protected Clothing[] clothing;
    protected List<RegularItem> regularItems;
    protected BodyPart root;

    // Inventory
    protected Inventory inventory;

    // Stats
    protected int defense;

    // Movement n' Physics
    protected float accelerationScalar;
    protected float rotationAccelFactor;
    protected float maxSpeed;
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected new Rigidbody rigidbody;
    protected new CapsuleCollider collider;
    private Animator characterAnimator;
    private CrawlRootAnim crawlController;
    private Quaternion crawlAngle;
    private float leanForwardAngle = 35f;
    private bool isCrawling;

    /// <summary>
    /// The distance between the bottom of the character collider and the ground
    /// in order to check if grounded or not.
    /// </summary>
    [SerializeField]
    private float groundedContact = .05f;

    // States n' Actions
    private bool isAlive;
    private CharacterState characterState;

    /// <summary>
    /// Get the inventory of this character.
    /// </summary>
    public Inventory Inventory { get { return inventory; } }

    protected bool IsAlive
    {
        get
        {
            return isAlive;
        }

        set
        {
            isAlive = value;
            
            if (isAlive == false)
            {
                //root.Detach();
                // set all bparts to false kinematics...
                BodyPart[] allBodyParts = GetComponentsInChildren<BodyPart>();
                for (int i = 0; i < allBodyParts.Length; ++i)
                {
                    allBodyParts[i].SetLimp();
                    allBodyParts[i].gameObject.layer = 0;
                }
            }
        }
    }

    /// <summary>
    /// Gets the body parts currently attached to this character.
    /// </summary>
    public BodyPart[] BodyParts
    {
        get
        {
            return joints.Values.Where(j => j.BodyPart != null).Select(j => j.BodyPart).ToArray();
        }
    }

    /// <summary>
    /// Gets whether or not this character is currently grounded.
    /// NOTE: If this isnt working, it's because there ISNT A GAME MANAGER IN THE SCENE!
    /// </summary>
    public bool IsGrounded
    {
        get
        {
           // Debug.DrawLine(transform.position, transform.position + (Vector3.down * collider.bounds.extents.y) + (Vector3.down * groundedContact), Color.black);
            //Debug.DrawLine(collider.bounds.center, collider.bounds.center + (Vector3.down * (collider.bounds.extents.y + groundedContact)), Color.black);
            //Debug.Log(collider.bounds.extents.y);
            //return Physics.CheckCapsule(collider.bounds.center, new Vector3(collider.bounds.center.x, collider.bounds.center.y + collider.height / 2, collider.bounds.center.z), collider.radius, GameManager.GroundedLayerMask);
            return Physics.Raycast(collider.bounds.center, Vector3.down, collider.bounds.extents.y + groundedContact, GameManager.GroundedLayerMask);
        }
    }

    // actions go here...

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected virtual void Awake()
    {
        // Grab all children with BodyPart scripts and store them for reference
        // Q: Is the head/torso a body part with inf health?
        joints = GetComponentsInChildren<CustomJoint>().ToDictionary(x => x.JointType, x => x);

        // grab all clothing currently on the character
        clothing = GetComponentsInChildren<Clothing>();

        // get all body parts that are children and set joint relationships ONCE
        // all other joint relationship will be handled in attach and detach

        // get root and set its skeleton
        foreach (Transform possBodyPart in transform)
            if ((root = possBodyPart.GetComponent<BodyPart>()) != null)
                break;

        root.InitSkeleton(joints);

        // init items in hand
        regularItems = new List<RegularItem>();

        // init this characters inventory
        inventory = GetComponent<Inventory>();

        // init defense modifier
        defense = 10;

        // init physics defaults
        accelerationScalar = 1.25f;
        rotationAccelFactor = .01f;
        maxSpeed = 2.0f;
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        //rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
        // get the overall capsule collider
        collider = GetComponent<CapsuleCollider>();

        // calc bounds for the character
        //StartCoroutine(RecalculateCollisionBounds());
        RecalculateCollisionBounds();

        // always start alive
        isAlive = true;
        isCrawling = false; // assume not crawling until crawl check
        characterState = CharacterState.Idle;

        // set layer of character colliders to 10
        gameObject.layer = 10;

        // get character animator found in skeletal root!
        characterAnimator = GetComponentInChildren<Animator>();
        characterAnimator.SetInteger("characterState", (int)characterState);
        crawlController = GetComponentInChildren<CrawlRootAnim>();
        crawlAngle = Quaternion.Euler(leanForwardAngle, 0, 0);
    }

    protected virtual void Start()
    {
        // check to see if we should start as crawling.
        CrawlCheck();
    }

    private void ChangeStateTo(CharacterState state)
    {
        // if already set, just return
        if (state == characterState)
            return;

        // set state
        characterState = state;

        // make changes
        switch (state)
        {
            case CharacterState.None:
                break;

            case CharacterState.Idle:
                // set walk speed to 0
                characterAnimator.SetFloat("walkSpeed", 0f);

                // return to 0 angle.
                if (crawlController != null)
                    crawlController.SetCrawlingAngle(Quaternion.identity);
                break;

            case CharacterState.Walk:
                // return to crawl angle if crawling!
                if (crawlController != null && isCrawling)
                    crawlController.SetCrawlingAngle(crawlAngle);
                break;

            case CharacterState.Fall:
                break;
        }

        // set state in animator
        characterAnimator.SetInteger("characterState", (int)characterState);
    }

    protected virtual void Update ()
    {
        // apply character state actions.
        switch (characterState)
        {
            case CharacterState.None:
                break;

            case CharacterState.Idle:
                break;

            case CharacterState.Walk:
                characterAnimator.SetFloat("walkSpeed", rigidbody.velocity.magnitude);
                break;

            case CharacterState.Fall:
                break;
        }
    }

    void FixedUpdate()
    {
        // calculate movement
        ProcessMovement();

        // apply velocity changes to this character
        if (acceleration != Vector3.zero)
        {
            // TODO: When on slope, apply velocity parallel to slope with slope max.?
            velocity += acceleration * Time.fixedDeltaTime;

            // lerp directional velocity from this objects forward to the accel norm
            // creates rotation effect purely from velocity
            // TODO: looks smooth when not performing a 180
            //velocity = Vector3.MoveTowards(transform.forward, acceleration.normalized, Time.fixedDeltaTime * rotationAccelFactor) * velocity.magnitude;
            velocity = Vector3.LerpUnclamped(transform.forward, acceleration.normalized, Time.fixedDeltaTime * rotationAccelFactor) * velocity.magnitude;
        }
        else
        {
            // not sure if this is accurate???
            //velocity = Vector3.LerpUnclamped(velocity, Vector3.zero, Time.fixedDeltaTime * accelerationScalar);
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, Time.fixedDeltaTime * accelerationScalar);
        }

        // clamp to this characters max velocity.
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // makes character face velocity.
        if (velocity != Vector3.zero)
            transform.forward = velocity.normalized;

        // grounded check
        if (IsGrounded)
        {
            rigidbody.velocity = new Vector3(velocity.x, rigidbody.velocity.y, velocity.z);
            if (velocity.sqrMagnitude > .16f)
                ChangeStateTo(CharacterState.Walk);
            else
                ChangeStateTo(CharacterState.Idle);
        }
        else
            ChangeStateTo(CharacterState.Fall);

        // set accel to 0
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Use this function for character specific movement.
    /// i.e. player is controlled via input but AI will move based on rules.
    /// </summary>
    abstract protected void ProcessMovement(); 
    
    /// <summary>
    /// Mounts an item to the first open mount point.
    /// </summary>
    /// <param name="itemToMount">Item to mount to this character.</param>
    /// <returns>True if the item mounted successfully, otherwise false.</returns>
    public bool MountItem(RegularItem itemToMount)
    {
        // look for empty mount point
        MountPoint[] mountPoints = GetComponentsInChildren<MountPoint>();

        // if slots are full, return false!
        for (int i = 0; i < mountPoints.Length; ++i)
            if (itemToMount.MountTo(mountPoints[i]) != null)
                return true;

        // return true, we did it reddit!
        return false;
    }

    /// <summary>
    /// Mounts a given item to a provided body part type if that joint's mount
    /// point is empty.
    /// </summary>
    /// <param name="itemToMount">Item to mount to this character.</param>
    /// <param name="bodyPartType">Body part ID to mount this item to.</param>
    /// <returns>True if the item mounted successfully, otherwise false.</returns>
    public bool MountItem(RegularItem itemToMount, int bodyPartType)
    {
        // look for empty mount point
        MountPoint[] mountPoints = GetComponentsInChildren<MountPoint>();

        // if slots are full, return false!
        for (int i = 0; i < mountPoints.Length; ++i)
            if (mountPoints[i].BodyPartType == bodyPartType)
                return itemToMount.MountTo(mountPoints[i]) ? true : false;

        // return true, we did it reddit!
        return false;
    }

    public bool Attach(BodyPart bodyPartToAttach)
    {
        if (bodyPartToAttach == null || !joints.ContainsKey(bodyPartToAttach.BodyPartType))
            return false;
        
        if (!bodyPartToAttach.SetSkeleton(joints))
            return false;

        // get children bparts
        BodyPart[] bodyParts = bodyPartToAttach.GetComponentsInChildren<BodyPart>();

        // attempt to add colliders to clothing
        for (int i = 0; i < clothing.Length; ++i)
        {
            // TMP FIX: get all component bparts, do this recursively??
            for (int z = 0; z < bodyParts.Length; ++z)
                clothing[i].AddBodyPart(bodyParts[z]);
        }

        // set layer to 9 for all bparts
        for (int i = 0; i < bodyParts.Length; ++i)
            bodyParts[i].gameObject.layer = 9;

        // store bottom height to get relative height.
        Vector3 bottomHeight = collider.bounds.center - (transform.up * ((collider.height / 2) - collider.radius));

        // calc new bounds
        RecalculateCollisionBounds();

        // capsule check to see if we can move up.
        RaycastHit[] hits = Physics.CapsuleCastAll(bottomHeight,
            bottomHeight + (transform.up * (collider.height - (collider.radius * 2))),
            collider.radius - .1f,
            transform.up,
            collider.radius - .1f,
            GameManager.GroundedLayerMask);

        // TODO: set up raycast in a place that wont require detachment to be called.
        // revert if hits found.
        if (hits.Length != 0)
        {
            /*
            foreach (RaycastHit hit in hits)
                Debug.Log(hit.transform.name);
            Debug.DrawLine(bottomHeight, bottomHeight + (transform.up * (collider.height - (collider.radius * 2))));
            Debug.Break();
            */
            Detach(bodyPartToAttach.BodyPartType);
            return false;
        }

        CrawlCheck();

        return true;
    }

    /// <summary>
    /// Detaches a body part based on its index within the list of body parts.
    /// </summary>
    /// <param name="bodyPart">Index location of the body part to detach.</param>
    /// <returns>Reference to body part detached if it exists.</returns>
    public BodyPart Detach(int bodyPartID)
    {
        // see if part exists
        if (!joints.ContainsKey(bodyPartID))
        {
            Debug.Log("Failed: " + joints[bodyPartID].name);
            return null;
        }

        // store in a variable
        BodyPart tmpPart = joints[bodyPartID].BodyPart;

        if (tmpPart == null)
            return null;

        // set as active in the world.
        if (tmpPart.Detach() == null)
            return null;

        // adjust capsule collider pivot to new center and height
        RecalculateCollisionBounds();

        // remove from clothing array
        // attempt to add colliders to clothing
        for (int i = 0; i < clothing.Length; ++i)
        {
            // TMP FIX: get all component bparts, do this recursively??
            BodyPart[] bodyParts = tmpPart.GetComponentsInChildren<BodyPart>();
            for (int z = 0; z < bodyParts.Length; ++z)
                clothing[i].RemoveBodyPart(bodyParts[z]);
        }

        CrawlCheck();

        return tmpPart;
    }

    /// <summary>
    /// Call this function when the character should be placed into a "death" state.
    /// </summary>
    /*
    public virtual void Die()
    {
        // explode and then destroy the character?
        state = CharacterState.Dead;

        // unparent the root from this character handler and then remove the character handler.
    }
    */

    /// <summary>
    /// Recalculates the capsule colliders bounds.
    /// TODO: IGNORE HANDS
    /// </summary>
    private void RecalculateCollisionBounds()
    {
        // preserve rotation, rotate to 0 and then put back.
        //Quaternion currLocalRot = transform.localRotation;
        
        //transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        // RECURSIVELY GO THROUGH ALL CHILDREN AND ENCAPSULATE
        // TODO: make this function better

        // Get first body part (doesnt matter which one...)
        Bounds initialBounds = root.GetComponent<MeshFilter>().mesh.bounds;

        RecalculateCollisionBounds(ref initialBounds, root.gameObject);

        collider.center = initialBounds.center;
        collider.radius = Mathf.Max(initialBounds.size.x / 2, initialBounds.size.z / 2);
        collider.height = initialBounds.size.y;

        // put back rotation
        //transform.localRotation = currLocalRot;
    }

    private void RecalculateCollisionBounds(ref Bounds currentBounds, GameObject currentBodyPart)
    {
        // make sure not a regular item (i.e. something that wont be ATTACHED like a bodypart)
        RegularItem regularItem = currentBodyPart.GetComponent<RegularItem>();
        if (regularItem != null)
            return;
        
        // QUICK FIX TODO DELETE
        BodyPart currBPart = currentBodyPart.GetComponent<BodyPart>();
        if (currBPart != null && (currBPart.BodyPartType == 1 || currBPart.BodyPartType == 5))
            return;

        if (currentBodyPart.GetComponent<Cloth>() != null)
            return;
        
        //Debug.Log("Added: " + currentBodyPart.name);
        // keep last local rot
        Quaternion currLocalRot = currentBodyPart.transform.localRotation;
        currentBodyPart.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (currentBodyPart.transform.childCount != 0)
            foreach (Transform child in currentBodyPart.transform) 
                RecalculateCollisionBounds(ref currentBounds, child.gameObject);

        // attempt to get MeshFilter
        // TODO OR ASK FOR MESHFILTER IN FUNCTION AS WE CAN DO MESHFILTER.TRANSFORM
        MeshFilter tmpMeshFilter = currentBodyPart.GetComponent<MeshFilter>();
        if (tmpMeshFilter == null)
            return;

        Bounds newBounds = currentBodyPart.GetComponent<MeshFilter>().mesh.bounds;

        // GET CENTER OF MESH
        // DIV BY LOCAL SCALE TO MOVE CENTER TO APPROPRIATE POSITION...
        // !!!!!! THIS IS WHY EVERYTHING SHOULD IMPORT AT 1 1 1 SCALE !!!!!!!
        // RENDERER CENTER BOUNDS ARE LOCAL
        // FIND A WAY TO GET TRUE CENTER ANOTHER WAY (this is because it is in world space and NOT local.
        //newBounds.center = transform.InverseTransformPoint(currentBodyPart.transform.TransformPoint(newBounds.center));
        newBounds.center = transform.InverseTransformPoint(currentBodyPart.transform.TransformPoint(newBounds.center));
        currentBounds.Encapsulate(newBounds);

        // put back rotation after all have been processed?
        currentBodyPart.transform.localRotation = currLocalRot;
    }

    /// <summary>
    /// Checks to see if the character is missing enough legs to crawl!
    /// </summary>
    private void CrawlCheck()
    {
        // determine if player should crawl, check to see if 3 and 7 are on the player.
        if (joints[3].BodyPart == null &&
            joints[7].BodyPart == null)
        {
            isCrawling = true;
            characterAnimator.SetBool("isCrawling", isCrawling);
            if(crawlController != null)
                crawlController.SetCrawlingAngle(crawlAngle);
        }
        else
        {
            if (isCrawling)
            {
                isCrawling = false;
                characterAnimator.SetBool("isCrawling", isCrawling);
                if (crawlController != null)
                    crawlController.SetCrawlingAngle(Quaternion.identity);
            }
        }
    }
}