using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Spear must contain capsule collider for pass through information.
/// </summary>
[RequireComponent(typeof(CapsuleCollider))]
public class Spear : Weapon {
    // Fields
    [SerializeField]
    private ImpalePoint impalePoint = null;
    private List<GameObject> currColls;
    private CapsuleCollider passThroughTrigger;
    private CapsuleCollider capsuleCollider;
        
    protected override void Awake()
    {
        base.Awake();

        type = RegularItemType.Weapon;
        amountOfItems = 1;
        damage = 10;
        capsuleCollider = collider as CapsuleCollider;
        passThroughTrigger = gameObject.AddComponent<CapsuleCollider>();
        passThroughTrigger.center = capsuleCollider.center;
        passThroughTrigger.radius = capsuleCollider.radius;
        passThroughTrigger.height = capsuleCollider.height;
        passThroughTrigger.direction = capsuleCollider.direction;
        passThroughTrigger.isTrigger = true;
        passThroughTrigger.enabled = false;

        impalePoint.OnImpale += ImpalePoint_OnImpale;
        impalePoint.AfterImpale += ImpalePoint_AfterImpale;

        currColls = new List<GameObject>();
    }

    private void ImpalePoint_OnImpale(Collision expectedObject)
    {
        // enable pass through trigger check
        passThroughTrigger.enabled = true;

        // add the last collision to current collisions
        currColls.Clear();
        currColls.Add(expectedObject.gameObject);

        // make sure what we hit was an ATTACHED body part.
        BodyPart collidedBodyPart;
        if ((collidedBodyPart = expectedObject.collider.GetComponent<BodyPart>()) != null &&
            collidedBodyPart.Joint != null)
        {
            // remove health based on damage
            collidedBodyPart.Health -= damage;
        }
    }

    private void ImpalePoint_AfterImpale(Collision expectedObject)
    {
        // disable pass through collision check.
        passThroughTrigger.enabled = false;
    }
    
    // Use this for initialization
    void Start () {
        // test force
        rigidbody.AddRelativeForce(Vector3.right * 500);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override RegularItem MountTo(MountPoint mountPoint)
    {
        Debug.Log("Status: " + impalePoint.IsActive);
        if (!impalePoint.IsActive)
        {
            //rigidbody.velocity = Vector3.zero;
            // reset everything.
            // set to active and apply force
            impalePoint.Reset();

            return base.MountTo(mountPoint);
        }

        return null;
    }


    public override void Use()
    {
        if (impalePoint.IsActive)
        {
            // set physicsy
            impalePoint.SetActive();

            // unparent
            transform.parent = null;

            // apply force relative
            rigidbody.AddRelativeForce(Vector3.right * 800);
        }
    }

    void FixedUpdate()
    {
        // check to see if the impale point original target
        // is no longer currently colliding
        if (impalePoint.LastCollision != null &&
            !currColls.Contains(impalePoint.LastCollision.gameObject))
        {
            // if no longer colliding, disable trigger and reset!
            impalePoint.Reset();
            impalePoint.SetActive();
            passThroughTrigger.enabled = false;
            currColls.Clear();
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.GetComponent<ImpalePoint>() == null &&
            !currColls.Contains(collider.gameObject))
            currColls.Add(collider.gameObject);
    }

    public void OnTriggerExit(Collider collider)
    {
        if (currColls.Contains(collider.gameObject))
            currColls.Remove(collider.gameObject);
    }

}
