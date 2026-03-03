using UnityEngine;

public class GorillaBalloonState : BaseState
{
    Gorilla _myGorilla;

    public GorillaBalloonState(Gorilla gori)
    {
        _myGorilla = gori;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myGorilla.currentState = FSMStates.StunnedState;
        _myGorilla.ChangeAnimation(_myGorilla._destroyBallon);
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
