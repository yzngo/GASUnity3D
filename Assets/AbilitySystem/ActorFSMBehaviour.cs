﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorFSMBehaviour : StateMachineBehaviour 
{
    public class StateMechineEvent : UnityEvent<Animator, int> {}
    public class StateEvent : UnityEvent<Animator, AnimatorStateInfo, int> {}

    public StateMechineEvent StateMachineEnter = new StateMechineEvent();
    public StateMechineEvent StateMachineExit = new StateMechineEvent();

    public StateEvent StateEnter = new StateEvent();
    public StateEvent StateUpdate = new StateEvent();
    public StateEvent StateExit = new StateEvent();
    public StateEvent StateMove = new StateEvent();
    public StateEvent StateIK = new StateEvent();


    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        StateEnter.Invoke(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        StateUpdate.Invoke(animator, stateInfo, layerIndex);
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        StateExit.Invoke(animator, stateInfo, layerIndex);
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        StateMove.Invoke(animator, stateInfo, layerIndex);
    }

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        StateIK.Invoke(animator, stateInfo, layerIndex);
    }

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        StateMachineEnter.Invoke(animator, stateMachinePathHash);
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        StateMachineExit.Invoke(animator, stateMachinePathHash);
    }
}

