﻿/*
    Spear
    ------------------------
    AUTHS Douglas Gliner
    
*/

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
    //private List<Collider> currColls;
    private CapsuleCollider passThroughTrigger;
    private CapsuleCollider capsuleCollider;

    private static ItemAnimationInfo leftHand = new ItemAnimationInfo(
        "leftHandItemState",
        "aim_left",
        "throw_left",
        "equipSpearLeft",
        "throwing_left",
        "equipNothingLeft");

    private static ItemAnimationInfo rightHand = new ItemAnimationInfo(
        "rightHandItemState",
        "aim_right",
        "throw_right",
        "equipSpearRight",
        "throwing_left",
        "equipNothingRight");

    protected override void Awake()
    {
        base.Awake();

        ItemAnimation[CreatureBodyBones.Left_Arm_Part_2] = rightHand;
        ItemAnimation[CreatureBodyBones.Right_Arm_Part_2] = leftHand;

        type = RegularItemType.Weapon;
        amountOfItems = 1;
        damage = 35;
        UseAnimationOffset = .75f;

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
        
        //currColls = new List<Collider>();
    }

    private void ImpalePoint_OnImpale(Collider expectedObject)
    {
        // enable pass through trigger check
        passThroughTrigger.enabled = true;

        // add the last collision to current collisions
        //currColls.Clear();
        //currColls.Add(expectedObject);

        // make sure what we hit was an ATTACHED body part.
        BodyPart collidedBodyPart;
        if ((collidedBodyPart = expectedObject.GetComponent<BodyPart>()) != null &&
            collidedBodyPart.Joint != null)
        {
            // remove health based on damage
            collidedBodyPart.Health -= damage;
        }
    }

    private void ImpalePoint_AfterImpale(Collider expectedObject)
    {
        // disable pass through collision check.
        passThroughTrigger.enabled = false;
    }
    
    // Use this for initialization
    void Start () {
        // test force
        //rigidbody.AddRelativeForce(Vector3.right * 2000);
        //impalePoint.Reset();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Use()
    {
        if (impalePoint.ImpaleState != ImpaleState.Impaling)
        {
            // set physicsy
            impalePoint.SetActive();

            // unparent
            //transform.parent = null;
            Unmount();

            // apply force relative
            rigidbody.AddRelativeForce(Vector3.right * 800);
        }
    }

    void FixedUpdate()
    {
        // check to see if the impale point original target
        // is no longer currently colliding
        /*
        if (impalePoint.LastCollision != null &&
            !currColls.Contains(impalePoint.LastCollision))
        {
            Debug.Log("going out of " + impalePoint.LastCollision.name);
            // if no longer colliding, disable trigger and reset!
            impalePoint.Reset();
            impalePoint.SetActive();
            passThroughTrigger.enabled = false;
            currColls.Clear();
        }*/
    }

    /*
    public void OnTriggerEnter(Collider collider)
    {

        if (!currColls.Contains(collider))
        {
            Debug.Log("Entered " + collider.name);
            currColls.Add(collider);
            if (collider.GetComponent<Renderer>() != null)
                collider.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        }
        
        
    }
    */

    public void OnTriggerExit(Collider collider)
    {
        /*
        if (currColls.Contains(collider))
        {
            Debug.Log("Exited " + collider.name);
            currColls.Remove(collider);
            if (collider.GetComponent<Renderer>() != null)
                collider.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        */
        Debug.Log("exited " + collider.name);
        
    }

    protected override bool MountCheck()
    {
        if (impalePoint.ImpaleState != ImpaleState.Impaling)
        {
            //Debug.Log("checking " + name);
            //rigidbody.velocity = Vector3.zero;
            // reset everything.
            // set to active and apply force
            impalePoint.Reset();
            return true;
        }
        return false;
    }
}
