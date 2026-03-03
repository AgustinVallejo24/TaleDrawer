using UnityEngine;

public class GorillaIdleState : BaseState
{
    Gorilla _myGorilla;
    float timer;
    public GorillaIdleState(Gorilla gori)
    {
        _myGorilla = gori;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myGorilla.currentState = FSMStates.IdleState;
        timer = 0;
        _myGorilla.ChangeAnimation(_myGorilla._idleT);
    }

    public override void OnExit()
    {
        
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
        if (timer < 1.2f)
        {
            timer += Time.deltaTime;

        }
        else
        {
            _myGorilla._fsm.ChangeState(FSMStates.PatrollState);
        }
    }
    
}
