using UnityEngine;
using System.Collections;

/// <summary>
/// Every projectile must have a collider.
/// </summary>
[RequireComponent(typeof(Collider))]

/// <summary>
/// Colliders require rigidbodies.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ImpalePointV2 : MonoBehaviour {

    // Fields
    private Collision finalCollision;
    private Transform parentModel;
    private Rigidbody parentRigidBody;
    private new Rigidbody rigidbody;
    private Vector3 lastVelocity;
    //private Vector3 projection;
    private ImpaleState impaleState;
    private float minVelocity = 9f;
    //private float impaleDistance = 0f;
    private float collidedMass;
    private float lerpTime = 1f;
    private float currentLerpTime;
    private float currSpeed;

    public delegate void Action(Collision collision);
    public event Action OnImpale;
    public event Action AfterImpale;

    /// <summary>
    /// Get or set whether or not this projectile should be looking to respond to collision.
    /// </summary>
    protected bool IsActive { get; set; }

    /// <summary>
    /// Get the last collision obtained from the impale point.
    /// </summary>
    public Collision LastCollision { get { return finalCollision; } }

    // Properties
    /// <summary>
    /// Get or set the distance you want this object to sink into after contact.
    /// </summary>
    //public float ImpaleDistance { get; set; }

    void Awake()
    {
        IsActive = true;
        rigidbody = GetComponent<Rigidbody>();

        // freeze constraints
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // dont use gravity
        rigidbody.useGravity = false;

        // Error check to see if parent is null
        if ((parentModel = transform.parent) == null)
            Debug.LogError("ERROR: No required parent set for " + name);

        // Get parent rigidbody and error if its not there.
        if ((parentRigidBody = parentModel.GetComponent<Rigidbody>()) == null)
            Debug.LogError("ERROR: " + transform.parent.name + " doesnt have a rigidbody!");

        // ignore collision with parent
        Physics.IgnoreCollision(GetComponent<Collider>(), parentModel.GetComponent<Collider>());

        // Default to no impale state
        impaleState = ImpaleState.None;

        // set layer to 9
        gameObject.layer = 9;
    }

    void FixedUpdate()
    {
        // react based on state
        switch (impaleState)
        {
            case ImpaleState.None:
                lastVelocity = parentRigidBody.velocity;
                break;

            case ImpaleState.Impaling:
                parentRigidBody.drag = collidedMass;

                // go until embedded
                if (parentRigidBody.velocity.magnitude <= .05f)
                {
                    // reset velocities
                    parentRigidBody.velocity = Vector3.zero;
                    lastVelocity = Vector3.zero;

                    parentRigidBody.isKinematic = true;
                    parentRigidBody.useGravity = true;

                    // call after impale event
                    if (AfterImpale != null)
                        AfterImpale(finalCollision);

                    // set to embedded
                    impaleState = ImpaleState.Embedded;
                }
                break;

            case ImpaleState.Embedded:
                break;
        }
    }

    // after traveling for speicifed distance from contact point
    // stop
    void OnCollisionEnter(Collision collision)
    {
        if (IsActive && collision.collider.GetComponent<ImpalePoint>() == null)
        {
            // TO-DO: slow speed? dont let it attach!!
            // if velocity is negative direction of forward, return
            // if speed is too little to impale, return.
            //if (Vector3.Dot(projection, parentModel.right) < 0 ||
            //    (projection = Vector3.Project(parentRigidBody.velocity, parentModel.TransformDirection(parentModel.right))).sqrMagnitude < 10)
            Debug.Log(parentModel.name + " collided with " + collision.collider.name);
            if (Vector3.Dot(lastVelocity, parentModel.right) < minVelocity)
            {
                return;
            }

            // ignore collision w/ collided
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            Physics.IgnoreCollision(collision.collider, parentModel.GetComponent<Collider>());

            // set parent
            parentModel.parent = collision.transform;

            // stop checking for OnCollisionEnter
            IsActive = false;

            // go kinematic and stop detecting collision
            rigidbody.isKinematic = true;
            parentRigidBody.useGravity = false;

            // set impale state to currently impaling
            impaleState = ImpaleState.Impaling;

            // set final collision
            finalCollision = collision;

            // see if collided object has a rigibody attachement.
            Rigidbody collidedRigidbody;
            if ((collidedRigidbody = finalCollision.collider.GetComponent<Rigidbody>()) != null)
                collidedMass = collidedRigidbody.mass;
            else
                collidedMass = 100f;

            // call on impale event
            if (OnImpale != null)
                OnImpale(finalCollision);
        }
    }

    // reset and set active.
    public void ResetAndSetActive()
    {
        // reset state info
        IsActive = true;
        impaleState = ImpaleState.None;
        rigidbody.isKinematic = false;
        parentRigidBody.isKinematic = false;
        currentLerpTime = 0;

        // set last velocity back
        lastVelocity = parentModel.right * currSpeed;
        currSpeed = 0;
        rigidbody.velocity = lastVelocity;
        Debug.Log(lastVelocity);

        // if collision info is stored, detection collision again with that object
        Physics.IgnoreCollision(finalCollision.collider, GetComponent<Collider>());
        Physics.IgnoreCollision(finalCollision.collider, parentModel.GetComponent<Collider>());

        // unparent
        parentModel.parent = null;

        // set final collision null
        finalCollision = null;
    }
}
