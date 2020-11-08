using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTalkerText : StateMachineBehaviour
{
    public string text;
    public float duration;

    float timer;
    bool timerZero = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("proceed");
        DialogueManager.SetTalkerText(text);
        timer = duration;
        timerZero = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer -= Time.deltaTime;
        if ((timer < 0 || (Input.GetKeyDown(KeyCode.Space) && DialogueManager.currentTalker && DialogueManager.currentTalker.readyToSkip)) && !timerZero)
        {
            timerZero = true;
            DialogueManager.SetTalkerText("");
            animator.SetTrigger("proceed");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("proceed");

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
