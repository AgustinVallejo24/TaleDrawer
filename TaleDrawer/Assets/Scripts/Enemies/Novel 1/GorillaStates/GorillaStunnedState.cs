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

        if(_myGorilla.myGem != null)
        {
            _myGorilla.myGem.myCollider.enabled = true;
        }
    }

    public override void OnExit()
    {
        if (_myGorilla.myGem != null)
        {
            _myGorilla.myGem.myCollider.enabled = false;
        }
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
        
    }
    
}
