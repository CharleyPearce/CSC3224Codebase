using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationScripts : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player script = animator.GetComponent<Player>();
        script.attack(new Vector3(1, 0, 0));
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Attack", false);
    }
}
