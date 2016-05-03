/*
    Corpse Class
    ------------------------
    AUTHS Douglas Gliner
    TO-DO
    NOTES
        - Used for objects in the scene that are spawned as dead and will never
          ever move.
*/
using UnityEngine;
using System.Collections;

public class Corpse : Character
{

    /*
        Use this function as the "constructor" since it occurs at the same time
        as instantiation.
    */
    protected override void Awake()
    {
        // Initiate parent first THEN move on to this classes stuff.
        base.Awake();

        // tag as corpse for easy mouse detection
        tag = "Corpse";
    }

    // Use this for initialization
    protected override void Start()
    {
        //IsAlive = false;
        Die();
    }

    // Update is called once per frame
    protected override void Update()
    { 
        base.Update();
    }

    protected override void Die()
    {
        base.Die();

        // set all bparts to false kinematics...
        BodyPart[] allBodyParts = GetComponentsInChildren<BodyPart>();
        for (int i = 0; i < allBodyParts.Length; ++i)
        {
            allBodyParts[i].SetLimp();
            allBodyParts[i].gameObject.layer = 0;
        }
    }

    /// <summary>
    /// Corpses dont move...
    /// </summary>
    protected override void ProcessMovement() { }
}
