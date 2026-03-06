using UnityEngine;

public class GorillaPatrollingState : BaseState
{
    Gorilla _myGorilla;
    Character character;
    public GorillaPatrollingState(Gorilla gori, Character chara)
    {
        _myGorilla = gori;
        character = chara;            
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myGorilla.currentState = FSMStates.PatrollState;
        if (_myGorilla.climbing)
        {
            _myGorilla.ChangeAnimation(_myGorilla._moveT2);
        }
        else
        {
            _myGorilla.ChangeAnimation(_myGorilla._moveT);
        }
            
    }

    public override void OnExit()
    {
        
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
        if (_myGorilla.climbing)
        {
            if (Mathf.Abs(_myGorilla.transform.position.x - _myGorilla.upperPatrollingNodes[_myGorilla.currentUpperNodeIndex].transform.position.x) > .1f)
            {
                _myGorilla.Move(-Mathf.Sign(_myGorilla.transform.position.x - _myGorilla.upperPatrollingNodes[_myGorilla.currentUpperNodeIndex].transform.position.x), _myGorilla.climbingSpeed);
            }
            else
            {
                _myGorilla.entityRigidbody.linearVelocity = Vector2.zero;
                if (_myGorilla.currentUpperNodeIndex == _myGorilla.upperPatrollingNodes.Length - 1)
                {
                    _myGorilla.currentUpperNodeIndex = 0;
                }
                else
                {
                    _myGorilla.currentUpperNodeIndex++;
                }
                _myGorilla._fsm.ChangeState(FSMStates.IdleState);
            }
        }
        else
        {
            if (Mathf.Abs(_myGorilla.transform.position.x - _myGorilla.patrollingNodes[_myGorilla.currentNodeIndex].transform.position.x) > .1f)
            {
                _myGorilla.Move(-Mathf.Sign(_myGorilla.transform.position.x - _myGorilla.patrollingNodes[_myGorilla.currentNodeIndex].transform.position.x));
            }
            else
            {
                _myGorilla.entityRigidbody.linearVelocity = Vector2.zero;
                if (_myGorilla.currentNodeIndex == _myGorilla.patrollingNodes.Length - 1)
                {
                    _myGorilla.currentNodeIndex = 0;
                }
                else
                {
                    _myGorilla.currentNodeIndex++;
                }
                _myGorilla._fsm.ChangeState(FSMStates.IdleState);
            }
        }
        
    }
}
