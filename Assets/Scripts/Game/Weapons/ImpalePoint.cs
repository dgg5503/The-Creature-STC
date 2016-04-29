using UnityEngine;
using System.Collections;
using System;

public enum ImpaleState
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
    public bool IsActive { get; protected set; }

    /// <summary>
    /// Get the last collision obtained from the impale point.
    /// </summary>
    public Collision LastCollision { get { return finalCollision; } }

    public ImpaleState ImpaleState { get { return impaleState; } }

    // Properties
    /// <summary>
    /// Get or set the distance you want this object to sink into after contact.
    /// </summary>
    //public float ImpaleDistance { get; set; }

    void Awake()
    {
        IsActive = true; // WAS TRUE!
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
        switch(impaleState)
        {
            case ImpaleState.None:
                lastVelocity = parentRigidBody.velocity;
                //projection = Vector3.Project(parentRigidBody.velocity, parentModel.TransformDirection(parentModel.right));
                
                //Debug.Log(Vector3.Dot(parentRigidBody.velocity, parentModel.right));
                //Debug.Log(Vector2.Dot(projection.normalized, Vector3.forward));
                //Debug.DrawLine(parentModel.position, parentModel.position + projection, Color.black);
                //Debug.DrawLine(parentModel.position, parentModel.position + parentModel.right * 10, Color.red);
                break;

            case ImpaleState.Impaling:
                // thanks to: https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
                //increment timer once per frame
                currentLerpTime += Time.fixedDeltaTime;
                if (currentLerpTime > lerpTime)
                    currentLerpTime = lerpTime;

                // below float value is dependent on the density of the penetrating object.
                currSpeed = Mathf.Lerp(lastVelocity.magnitude, 0, (currentLerpTime / lerpTime) * collidedMass);

                // newPosition = lerp(contactpoint, contactpoint + (lastVelocity.forward * desiredDistance), currentLerpTime / lerpTime
                // if position = endPoint
                // end action.

                //parentRigidBody.MovePosition(Vector3.zero);
                //parentRigidBody.mov
                //parentRigidBody.position += (parentRigidBody.transform.right * currSpeed) * Time.fixedDeltaTime;
                // below works
                //parentModel.localPosition += (parentRigidBody.transform.right * currSpeed) * Time.fixedDeltaTime;
                
                parentModel.localPosition += (parentModel.parent.InverseTransformDirection(parentModel.right) * currSpeed) * Time.fixedDeltaTime;
                //Debug.Log(currSpeed + " : " + parentRigidBody.position);

                // go until embedded
                if (currSpeed <= .05f)
                {
                    // reset velocities
                    parentRigidBody.velocity = Vector3.zero;
                    lastVelocity = Vector3.zero;

                    // reset lerp
                    currentLerpTime = 0;

                    // run backwards into enemies at full speed to kill them!
                    //parentRigidBody.detectCollisions = true;

                    // call after impale event
                    if (AfterImpale != null)
                        AfterImpale(finalCollision);

                    // set to embedded
                    //Debug.Log(transform.parent.name + " is impaled.");
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
           
            if (Vector3.Dot(lastVelocity, parentModel.right) < minVelocity)
            {
                return;
            }
            Debug.Log(parentModel.name + " collided with " + collision.collider.name);

            // ignore collision w/ collided
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            Physics.IgnoreCollision(collision.collider, parentModel.GetComponent<Collider>());

            // set parent
            parentModel.parent = collision.transform;

            // stop checking for OnCollisionEnter
            IsActive = false;
            Debug.Log(transform.parent.name + " is active: " + IsActive);
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
                collidedMass = 50f;

            // call on impale event
            if (OnImpale != null)
                OnImpale(finalCollision);
        }
    }

    // reset and set active.
    public void Reset()
    {
        if (impaleState != ImpaleState.Impaling)
        {
            // reset state info
            IsActive = true;
            impaleState = ImpaleState.None;
            rigidbody.isKinematic = true;
            parentRigidBody.isKinematic = true;
            currentLerpTime = 0;

            // set last velocity back
            lastVelocity = parentModel.right * currSpeed;
            parentRigidBody.velocity = lastVelocity;
            currSpeed = 0;

            //Debug.Log(rigidbody.velocity);

            // set final collision null
            //finalCollision = null;
        }
    }

    public void SetActive()
    {
        if (impaleState != ImpaleState.Impaling)
        {
            Debug.Log("SetActive called on " + transform.parent.name);

            // if collision info is stored, detection collision again with that object
            if (finalCollision != null)
            {
                Physics.IgnoreCollision(finalCollision.collider, GetComponent<Collider>(), false);
                Physics.IgnoreCollision(finalCollision.collider, parentModel.GetComponent<Collider>(), false);
            }

            // set final collision null
            finalCollision = null;

            // unparent
            parentModel.parent = null;

            // enable physics
            rigidbody.isKinematic = false;
            parentRigidBody.isKinematic = false;
        }
    }
}
