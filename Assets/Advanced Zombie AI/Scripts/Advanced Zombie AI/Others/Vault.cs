using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FMS_AdvancedZombieAI
{
    public class Vault : StateMachineBehaviour
    {
        float speed;
        NavMeshAgent agent;
        AdvancedZombieAI zombieAI;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            agent = animator.GetComponent<NavMeshAgent>();
            speed = agent.speed;
            agent.speed = 0.5f;
            zombieAI = animator.GetComponent<AdvancedZombieAI>();

            zombieAI.animator.SetLayerWeight(1, 0);
            zombieAI.animator.SetLayerWeight(2, 0);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            agent.speed = 1f;
            zombieAI.animator.SetLayerWeight(1, 0);
            zombieAI.animator.SetLayerWeight(2, 0);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            zombieAI.animator.SetLayerWeight(1, 1);
            zombieAI.animator.SetLayerWeight(2, 1);
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
}
