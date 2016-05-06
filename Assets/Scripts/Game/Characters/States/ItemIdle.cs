using UnityEngine;
using System.Collections;
using System;

public class ItemIdle : ItemStates
{
    public override void Enter(Character character, RegularItem regularItem, int bodyPartID)
    {
        // set fields
        this.character = character;
        this.regularItem = regularItem;
        this.bodyPartID = bodyPartID;

        // do animation stuff
        //Debug.Log("STARTING AT THE IDLE");
        character.CharacterAnimator.SetTrigger(
            regularItem.ItemAnimation[bodyPartID].equipTrigger);
        character.CharacterAnimator.SetInteger(
            regularItem.ItemAnimation[bodyPartID].itemState,
            (int)ItemState.Idle);
    }

    public override void Exit()
    {
        // do animation stuff
    }

    public override ItemStates HandleInput(KeyState keyState)
    {
        if (keyState == KeyState.KEY_DOWN)
        {
            //Debug.Log("go to aim");
            nextState = CreateInstance<ItemAim>();
            nextState.Enter(character, regularItem, bodyPartID);
            return nextState;
        }
        return this;
    }
}
