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

        // do animation stuff
        character.CharacterAnimator.SetInteger(
            regularItem.ItemAnimation[bodyPartID].itemState,
            (int)ItemState.Executing);
    }

    public override void Exit()
    {
        // do animation stuff
        // unregister with animator
    }

    public override ItemStates HandleInput(KeyState keyState)
    {
        //BreakState();
        Debug.Log("FIRE!");
        regularItem.Use();

        return null;
    }
}
