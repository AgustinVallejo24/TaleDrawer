using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public FSM fsm;
    
    public abstract void OnExit();

    public abstract void OnEnter();

    public abstract void Update();

    public abstract void FixedUpdate();

    public abstract void Transitions();
}

public enum FSMStates
{
    IdleState,
    FleeState,
    AttackState,
    PursuitState,
    PFState,
    TauntState,
    StunnedState,
    StoppedState,    
    TalkingState,
    PausedState,   
    Transition,    
    InitialState,    
    DeathState,    
    SleepingState,
    PatrollState,
    BalloonState
}
