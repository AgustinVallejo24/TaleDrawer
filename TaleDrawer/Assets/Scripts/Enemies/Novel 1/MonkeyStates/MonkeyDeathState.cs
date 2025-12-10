using System;
using UnityEngine;

public class MonkeyDeathState : BaseState
{
    Character _character;
    Monkey _myMonkey;

    public MonkeyDeathState(Monkey monk, Character chara)
    {
        _character = chara;
        _myMonkey = monk;
    }

    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myMonkey.currentState = FSMStates.DeathState;
        _myMonkey.DeathEvent();
    }   

    public override void OnExit()
    {
        
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
        
    }    
}
