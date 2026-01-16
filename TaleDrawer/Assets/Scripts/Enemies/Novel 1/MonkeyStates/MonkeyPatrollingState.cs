using UnityEngine;

public class MonkeyPatrollingState : BaseState
{
    Character _character;
    Monkey _myMonkey;
    public MonkeyPatrollingState(Monkey monk, Character chara)
    {
        _character = chara;
        _myMonkey = monk;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myMonkey.currentState = FSMStates.PatrollState;
    }

    public override void OnExit()
    {
        
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
      
       if(Mathf.Abs(_myMonkey.transform.position.x - _myMonkey.patrollingNodes[_myMonkey.currentNodeIndex].transform.position.x)>.1f)
        {        
            _myMonkey.Move(-Mathf.Sign(_myMonkey.transform.position.x - _myMonkey.patrollingNodes[_myMonkey.currentNodeIndex].transform.position.x));
        }
        else
        {
            _myMonkey.myRigidbody.linearVelocity = Vector2.zero;
            if(_myMonkey.currentNodeIndex == _myMonkey.patrollingNodes.Length - 1)
            {
                _myMonkey.currentNodeIndex = 0;
            }
            else
            {
                _myMonkey.currentNodeIndex++;
            }
            _myMonkey._fsm.ChangeState(FSMStates.IdleState);
        }
    }
}
