using UnityEngine;
using System.Collections;
using System;

enum ImpaleState
{
    None, // flying through air, sitting on the ground, etc...
    Impaling, // currently sinking into an object
    Embedded // within embedded object not moving.
}

/// <summary>
/// Every projectile must have a collider.
/// </summary>
[RequireComponent(typeof(Collider))]

/// <summary>
/// Colliders require rigidbodies.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ImpalePoint : MonoBehaviour {

    // Fields
    private Collision finalCollision;
    private Transform parentModel;
    private Rigidbody parentRigidBody;
    private new Rigidbody rigidbody;
    private Vector3 lastVelocity;
    private ImpaleState impaleState;
    private float collidedMass;
    private float currentSpeed;
    private float lerpTime = 1f;
    private float currentLerpTime;

    public delegate void Action(Collision collision);
    public event Action OnImpale;
    public event Action AfterImpale;

    /// <summary>
    /// Get or set whether or not this projectile should be looking to respond to collision.
    /// </summary>
    protected bool IsActive { get; set; }

    // Properties
    /// <summary>
    /// Get or set the distance you want this object to sink into after contact.
    /// </summary>
    //public float ImpaleDistance { get; set; }

    void Awake()
    {
        IsActive = true;
        rigidbody = GetComponent<Rigidbody>();

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
        switch(impaleState)
        {
            case ImpaleState.None:
                lastVelocity = parentRigidBody.velocity;
                break;

            case ImpaleState.Impaling:
                // thanks to: https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
                //increment timer once per frame
                currentLerpTime += Time.fixedDeltaTime;
                if (currentLerpTime > lerpTime)
                    currentLerpTime = lerpTime;

                // below float value is dependent on the density of the penetrating object.
                float currSpeed = Mathf.Lerp(lastVelocity.magnitude, 0, (currentLerpTime / lerpTime) * collidedMass);
                parentRigidBody.MovePosition(parentRigidBody.position + (parentRigidBody.transform.up * currSpeed) * Time.fixedDeltaTime);
                //parentRigidBody.position += (parentRigidBody.transform.up * currSpeed) * Time.fixedDeltaTime;
                //parentModel.localPosition += (parentRigidBody.transform.up * currSpeed) * Time.fixedDeltaTime;
                Debug.Log(lastVelocity.magnitude);

                // go until embedded
                if (currSpeed <= .05f)
                {
                    // reset velocities
                    parentRigidBody.velocity = Vector3.zero;
                    lastVelocity = Vector3.zero;

                    // reset lerp
                    currentLerpTime = 0;

                    // run backwards into enemies at full speed to kill them!
                    parentRigidBody.detectCollisions = true;

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
        if (IsActive)
        {
            // ignore collision w/ collided
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            Physics.IgnoreCollision(collision.collider, parentModel.GetComponent<Collider>());

            // set parent
            parentModel.parent = collision.transform;

            // stop checking for OnCollisionEnter
            IsActive = false;

            // go kinematic and stop detecting collision
            rigidbody.isKinematic = true;
            parentRigidBody.isKinematic = true;
            //rigidbody.detectCollisions = false;
            //parentRigidBody.detectCollisions = false;

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
}
