using UnityEngine;
using System.Collections;
using System;

public class SpearV2 : Projectile {

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
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public override void AfterImpale(Collider collider) { }

    public override void OnImpale(Collider collider)
    {
        // make sure what we hit was an ATTACHED body part.
        BodyPart collidedBodyPart;
        if ((collidedBodyPart = collider.GetComponent<BodyPart>()) != null &&
            collidedBodyPart.Joint != null)
        {
            // remove health based on damage
            collidedBodyPart.Health -= damage;
        }
    }

    public override void Use()
    {
        if (ImpaleState != ImpaleState.Impaling)
        {
            // set physicsy
            SetActive();

            // unparent
            Unmount();

            // apply force relative
            rigidbody.AddRelativeForce(Vector3.right * 800);
        }
    }

    protected override bool MountCheck()
    {
        if (ImpaleState != ImpaleState.Impaling)
        {
            Reset();
            return true;
        }
        return false;
    }

}
