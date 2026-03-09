using UnityEngine;

public class GorillaStunnedState : BaseState
{
    Gorilla _myGorilla;

    public GorillaStunnedState(Gorilla gori)
    {
        _myGorilla = gori;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myGorilla.currentState = FSMStates.StunnedState;
        _myGorilla.ChangeAnimation(_myGorilla._stunnedT);        
        _myGorilla.StartStun(_myGorilla.stunnedTime);
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
