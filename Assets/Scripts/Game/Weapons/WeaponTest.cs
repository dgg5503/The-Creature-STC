using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class WeaponTest : Item {

    // Physics
    private new Rigidbody rigidbody;
    private new Collider collider;

    // body part currently inside / collided with
    // what if INBETWEEN 2 bodyparts...
    BodyPart collidedBodyPart;

    protected override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        collidedBodyPart = null;
    }

    /// <summary>
    /// Used to see if this weapon collides with a body part.
    /// </summary>
    /// <param name="collided"></param>
    void OnCollisionEnter(Collision collided)
    {
        if (collidedBodyPart == null &&
            (collidedBodyPart = collided.gameObject.GetComponent<BodyPart>()) != null)
        {
            // apply damage and become "inactive"
            // set parent to the bpart
            transform.parent = collidedBodyPart.transform;

            // apply damage
            collidedBodyPart.Health -= 10;
            
            // turn off collision detection and ignore collision
            Physics.IgnoreCollision(collider, collided.collider);
            //rigidbody.detectCollisions = false;

            // turn on kinematic
            rigidbody.isKinematic = true;

            //rigidbody.dr
        }
    }

    // Use this for initialization
    void Start () {

        // test force
        rigidbody.AddRelativeForce(Vector3.up * 200);
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
