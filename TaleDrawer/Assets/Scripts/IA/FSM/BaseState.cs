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
    StunnedState2,
    TalkingState,
    PausedState,
    Idle2State,
    Phase1MultiShotState,
    Transition,
    Phase1SingleShotState,
    Phase1ShakeRoomAttackState,
    Phase1SuckAttackState,
    Phase1Attack4State,
    Phase2StompAttackState,
    Phase2JumpAttackState,
    Phase2JumpToCornerAttackState,
    Phase2MohgAttackState,
    Phase2SuckingWhileWalkingAttackState,
    InitialState,
    TiredState,
    DeathState,
    HidingState,
    EmergingState,
    Phase1ThornRain,
    PetalsAttack,
    ThornsAreaAttack,
    ShockWave,
    SummonAttack,
    SleepingState,
    TransformationState,
    LaserAttackState,
    ShootingLaserState,
    MoveForLaser,
    Shake,
}
