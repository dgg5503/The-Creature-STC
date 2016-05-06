using UnityEngine;
using System.Collections.Generic;

public enum AnimationState
{
    ENTER,
    UPDATE,
    EXIT
}

public class animationEventHandler : StateMachineBehaviour {
    // Delegate for callback type
    public delegate void AnimationCallback(AnimationState animationState, Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex);
    //public static readonly Dictionary<Animator, AnimationCallback> animationCallbacks = new Dictionary<Animator, AnimationCallback>();
    public static event AnimationCallback animationCallbacks;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        /*
        if (animationCallbacks.ContainsKey(animator))
            animationCallbacks[animator](AnimationState.ENTER, stateInfo, layerIndex);
            */
            
        if (animationCallbacks != null)
            animationCallbacks(AnimationState.ENTER, animator, stateInfo, layerIndex);
        base.OnStateEnter(animator, stateInfo, layerIndex);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        /*
        if (animationCallbacks.ContainsKey(animator))
            animationCallbacks[animator](AnimationState.UPDATE, stateInfo, layerIndex);*/
        if (animationCallbacks != null)
        {
            //Debug.Log(animationCallbacks.GetInvocationList().Length);
            animationCallbacks(AnimationState.UPDATE, animator, stateInfo, layerIndex);
        }
        base.OnStateUpdate(animator, stateInfo, layerIndex);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        /*
        if (animationCallbacks.ContainsKey(animator))
            animationCallbacks[animator](AnimationState.EXIT, stateInfo, layerIndex);*/
        if (animationCallbacks != null)
            animationCallbacks(AnimationState.EXIT, animator, stateInfo, layerIndex);
        base.OnStateExit(animator, stateInfo, layerIndex);
	}
}
