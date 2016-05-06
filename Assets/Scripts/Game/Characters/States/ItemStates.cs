using UnityEngine;
using System.Collections;

public abstract class ItemStates : ScriptableObject {
    protected enum ItemState
    {
        Idle,
        Aim,
        Executing
    }

    protected RegularItem regularItem;
    protected Character character;
    protected ItemStates nextState;
    protected int bodyPartID;

    public int BodyPartID { get { return bodyPartID; } }

    public abstract void Enter(Character character, RegularItem regularItem, int bodyPartID);
    
    public abstract void Exit();

    public abstract ItemStates HandleInput(KeyState keyState);

    protected virtual void AnimationCallback(AnimationState state, Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }


    /// <summary>
    /// Use this when you want to break out of any state.
    /// </summary>
    public void BreakState()
    {
        // set to idle
        character.CharacterAnimator.SetInteger(
            regularItem.ItemAnimation[bodyPartID].itemState,
            (int)ItemState.Idle);

        // unequip
        character.CharacterAnimator.SetTrigger(
            regularItem.ItemAnimation[bodyPartID].unequipTrigger
            );

        // unregister callback (must be handled by class child)
        //animationEventHandler.animationCallbacks -= AnimationCallback;
        Exit();

        // set item to null
        regularItem = null;
        DestroyImmediate(this);
    }

    /// <summary>
    /// Use this when you want item to return to idle.
    /// </summary>
    /// <returns></returns>
    /*
    public ItemStates BreakToIdle()
    {
        nextState = CreateInstance<ItemIdle>();
        nextState.Enter(character, regularItem, bodyPartID);
        return nextState;
    }
    */
}
