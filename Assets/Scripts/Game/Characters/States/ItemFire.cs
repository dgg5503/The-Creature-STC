using UnityEngine;
using System.Collections;
using System;

public class ItemFire : ItemStates
{
    public override void Enter(Character character, RegularItem regularItem, int bodyPartID)
    {
        // set fields
        this.character = character;
        this.regularItem = regularItem;
        this.bodyPartID = bodyPartID;

        // register with animator
        animationEventHandler.animationCallbacks += AnimationCallback;

        // do animation stuff
        character.CharacterAnimator.SetInteger(
            regularItem.ItemAnimation[bodyPartID].itemState,
            (int)ItemState.Executing);
    }

    public override void Exit()
    {
        // do animation stuff
        // unregister with animator
        animationEventHandler.animationCallbacks -= AnimationCallback;
        // return to idle
        /*
        character.CharacterAnimator.SetInteger(
            regularItem.ItemAnimation[bodyPartID].itemState,
            (int)ItemState.Idle);
            */
    }

    public override ItemStates HandleInput(KeyState keyState)
    {
        return null;
    }

    protected override void AnimationCallback(AnimationState state, Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator != character.CharacterAnimator)
            return;

        // ensure 
        if (character.CharacterAnimator.GetLayerIndex(regularItem.ItemAnimation[bodyPartID].layerName) == layerIndex)
        {
            // ensure 
            if (stateInfo.IsTag(regularItem.ItemAnimation[bodyPartID].throwState))
            {
                // wait until certain amount of time
                if ((stateInfo.normalizedTime * stateInfo.speed) >= (stateInfo.length * stateInfo.speed * regularItem.UseAnimationOffset))
                {
                    //Debug.Log("FIRE!");
                    //Debug.Log(stateInfo.normalizedTime + ": " + stateInfo.length + " and " + stateInfo.shortNameHash);

                    // match forward of creature
                    regularItem.transform.right = character.AimingAt; //character.transform.forward;
                    regularItem.Use();
                    Exit();
                    BreakState();
                    
                    //DestroyImmediate(this);
                }
            }
        }

        base.AnimationCallback(state, animator, stateInfo, layerIndex);
    }
}
