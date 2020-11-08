using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOptionsAnimator : StateMachineBehaviour
{
    public DialogueOptionAnimator[] animatorOptions;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("selectedOption", 0);

        List<DialogueOption> options = new List<DialogueOption>();
        int index = 0;
        foreach(DialogueOptionAnimator aopt in animatorOptions)
        {

            DialogueOption opt = new DialogueOption();
            opt.dialogue = aopt.optionText;
            opt.returnValue = index+1;
            index++;
            bool canAdd = true;
            if (aopt.showCondition != "")
            {
                char[] fullCond = aopt.showCondition.ToCharArray();
                string cond = "";
                for(int i = 0; i < fullCond.Length; i++)
                {
                    string character = fullCond[i].ToString();
                    if (character != "," && character != " ")
                        cond += character;
                    bool checkCond = i == fullCond.Length - 1 || character == ",";
                    if (!checkCond)
                        continue;
                    bool targetVal = cond.ToCharArray()[0].ToString() != "!";
                    if (!targetVal)
                        cond = cond.Substring(1);
                    if (animator.GetBool(cond) != targetVal)
                    {
                        canAdd = false;
                        break;
                    }
                    cond = "";
                }
            }
            if(canAdd)
            options.Add(opt);

        }
        if(options.Count == 0)
        {
            animator.SetBool("talking", false);
            return;
        }
        DialogueManager.SetDialogueOptions(animator, options.ToArray());
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.SetInteger("selectedOption", 0);
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
