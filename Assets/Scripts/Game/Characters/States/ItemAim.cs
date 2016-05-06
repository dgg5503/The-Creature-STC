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

        // register with animator
        animationEventHandler.animationCallbacks[character.CharacterAnimator] = AnimationCallback;

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
    }

    public override ItemStates HandleInput(KeyState keyState)
    {
        
        if (keyState == KeyState.KEY_UP &&
            animatorStateInfo.length != 0) // yield until aim state
        {
            Debug.Log(animatorStateInfo.normalizedTime + ": " + animatorStateInfo.length + " and " + animatorStateInfo.shortNameHash);
            if (animatorStateInfo.normalizedTime >= animatorStateInfo.length)
            {
                nextState = CreateInstance<ItemFire>();
                //Exit();
                nextState.Enter(character, regularItem, bodyPartID);
                
                Debug.Log("fire");
                return nextState;
            }
            else
            {
                nextState = CreateInstance<ItemIdle>();
                //Exit();
                nextState.Enter(character, regularItem, bodyPartID);
                Debug.Log("idle");
                return nextState;
            }
        }

        // WHAT TO DO ABOUT THIS?
        Debug.Log("this");
        return this;
    }

    protected override void AnimationCallback(AnimationState state, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ensure 
        if (stateInfo.IsTag("aim"))
            animatorStateInfo = stateInfo;
        base.AnimationCallback(state, stateInfo, layerIndex);
    }
}
