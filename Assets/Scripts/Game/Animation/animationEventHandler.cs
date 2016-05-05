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
    public delegate void AnimationCallback(AnimationState animationState, AnimatorStateInfo animatorStateInfo);
    public static readonly Dictionary<Animator, AnimationCallback> animationCallbacks = new Dictionary<Animator, AnimationCallback>();

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (animationCallbacks.ContainsKey(animator))
            animationCallbacks[animator](AnimationState.ENTER, stateInfo);
        base.OnStateEnter(animator, stateInfo, layerIndex);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (animationCallbacks.ContainsKey(animator))
            animationCallbacks[animator](AnimationState.UPDATE, stateInfo);
        base.OnStateUpdate(animator, stateInfo, layerIndex);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (animationCallbacks.ContainsKey(animator))
            animationCallbacks[animator](AnimationState.EXIT, stateInfo);
        base.OnStateExit(animator, stateInfo, layerIndex);
	}
}
