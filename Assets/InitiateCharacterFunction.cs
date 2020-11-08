using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateCharacterFunction : StateMachineBehaviour
{
    public ANIMSTATE_WHEN when;
    public string functionName;

    IInitiateFunctionFromAnimator c;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        c = animator.GetComponent<IInitiateFunctionFromAnimator>();
        if (c!= null && when == ANIMSTATE_WHEN.START)
            c.InitiateFunction(functionName);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (c != null && when == ANIMSTATE_WHEN.END)
            c.InitiateFunction(functionName);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
