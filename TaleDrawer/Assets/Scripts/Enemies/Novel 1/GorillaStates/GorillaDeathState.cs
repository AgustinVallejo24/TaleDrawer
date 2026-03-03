using UnityEngine;

public class GorillaDeathState : BaseState
{
    Character _character;
    Gorilla _myGorilla;

    public GorillaDeathState(Gorilla gori, Character chara)
    {
        _myGorilla = gori;
        _character = chara;
    }

    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myGorilla.currentState = FSMStates.DeathState;
        _myGorilla.DeathEvent();
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
