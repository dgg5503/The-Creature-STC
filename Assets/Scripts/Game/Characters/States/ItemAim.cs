using UnityEngine;
using System.Collections;
using System;

public class ItemAim : ItemStates
{
    //private bool checkingForTime = false;
    private AnimatorStateInfo animatorStateInfo;
    private AnimatorStateInfo zeroInfo = new AnimatorStateInfo();

    public override void Enter(Character character, RegularItem regularItem, int bodyPartID)
    {
        // set fields
        this.character = character;
        this.regularItem = regularItem;
        this.bodyPartID = bodyPartID;
        //Debug.Log("enter aim");
        // register with animator
        animationEventHandler.animationCallbacks += AnimationCallback;

        // do animation stuff
        //checkingForTime = false;
        character.CharacterAnimator.SetInteger(
            regularItem.ItemAnimation[bodyPartID].itemState,
            (int)ItemState.Aim);
    }

    public override void Exit()
    {
        // remove callback
        // register with animator
        //animationEventHandler.animationCallbacks[character.CharacterAnimator] = null;
        animationEventHandler.animationCallbacks -= AnimationCallback;
    }

    public override ItemStates HandleInput(KeyState keyState)
    {
        if (keyState == KeyState.KEY_UP) // yield until aim state
        {
            if (animatorStateInfo.length != 0 &&
                ((animatorStateInfo.normalizedTime * animatorStateInfo.speed) >= animatorStateInfo.length))
            {
                //Debug.Log("fire");
                nextState = CreateInstance<ItemFire>();
                Exit();
                nextState.Enter(character, regularItem, bodyPartID);
                //Debug.Break();
                //Debug.Log(animatorStateInfo.normalizedTime + ": " + animatorStateInfo.length + " and " + animatorStateInfo.shortNameHash);
                
                return nextState;
            }
            else
            {
                nextState = CreateInstance<ItemIdle>();
                Exit();
                nextState.Enter(character, regularItem, bodyPartID);
                //Debug.Log("idle");
                return nextState;
            }
        }

        // WHAT TO DO ABOUT THIS?
        //Debug.Log("this");
        //character.CalculateAimPoint();
        //character.transform.forward = character.AimingAt;
        return this;
    }

    protected override void AnimationCallback(AnimationState state, Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator != character.CharacterAnimator)
            return;

        if (stateInfo.IsTag(regularItem.ItemAnimation[bodyPartID].aimState) &&
        !character.CharacterAnimator.IsInTransition(layerIndex))
        {
            //Debug.Log(regularItem.ItemAnimation[bodyPartID].aimState);
            //Debug.Log((stateInfo.normalizedTime * stateInfo.speed) + " and " + stateInfo.length);
            animatorStateInfo = stateInfo;
        }
        else
            animatorStateInfo = zeroInfo;
        base.AnimationCallback(state, animator, stateInfo, layerIndex);
    }
}
