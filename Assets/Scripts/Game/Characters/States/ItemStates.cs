using UnityEngine;
using System.Collections;

public abstract class ItemStates : ScriptableObject {
    protected RegularItem regularItem;
    protected Character character;
    protected ItemStates nextState;
    protected int bodyPartID;

    public abstract void Enter(Character character, RegularItem regularItem, int bodyPartID);
    
    public abstract void Exit();

    public abstract ItemStates HandleInput(KeyState keyState);

    protected virtual void AnimationCallback(AnimationState state, AnimatorStateInfo stateInfo, int layerIndex) { }

    public ItemStates BreakState()
    {
        // set to idle
        character.CharacterAnimator.SetInteger(
            regularItem.ItemAnimation[bodyPartID].itemState,
            (int)ItemState.Idle);

        // unequip
        character.CharacterAnimator.SetTrigger(
            regularItem.ItemAnimation[bodyPartID].unequipTrigger
            );

        // unregister callback
        animationEventHandler.animationCallbacks.Remove(character.CharacterAnimator);

        // set item to null
        regularItem = null;
        return null;
    }
}
