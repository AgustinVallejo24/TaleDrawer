using System.Linq;

using UnityEngine;

public class MonkeyPursueState : BaseState
{
    Character _character;
    Monkey _myMonkey;
    bool isAttacking;    
    CustomNode newNode;

    public MonkeyPursueState(Monkey monk, Character chara)
    {
        _character = chara;
        _myMonkey = monk;
    }
    public override void FixedUpdate()
    {

    }

    public override void OnEnter()
    {
        _myMonkey.currentState = FSMStates.PursuitState;
        _myMonkey.SetSpeed(8f);
        _myMonkey.ChangeAnimation(_myMonkey._moveT);

    }

    public override void OnExit()
    {
        isAttacking = false;        
    }

    public override void Transitions()
    {

    }

    public override void Update()
    {
        
        if (Physics2D.OverlapBox(_myMonkey.transform.position, Vector2.one, 0, _myMonkey._playerMask) && !isAttacking)
        {
            isAttacking = true;
            newNode = _myMonkey.patrollingNodes.OrderBy(x => Vector3.Distance(_myMonkey.transform.position, x.transform.position)).First();
            _character.SendInputToFSM(CharacterStates.Stop);
            _character.characterRigidbody.linearVelocityX = 0;
                    
        }

        if (!isAttacking)
        {
            _myMonkey.Move(-Mathf.Sign(_myMonkey.transform.position.x - _character.transform.position.x));
        }
        else
        {
            if ((Mathf.Sign(newNode.transform.position.x - _myMonkey.transform.position.x) > 0))
            {
                _myMonkey.Move(-Mathf.Sign(_myMonkey.transform.position.x - (_character.transform.position.x - _myMonkey.attackDistance)));
                //Debug.LogError("Debug1: " + Vector2.Distance(new Vector2(_myMonkey.transform.position.x, 0), new Vector2((_character.transform.position.x - _myMonkey.attackDistance), 0)));
                if (Vector2.Distance(new Vector2(_myMonkey.transform.position.x, 0), new Vector2((_character.transform.position.x - _myMonkey.attackDistance), 0)) < 0.5f)
                {
                    _myMonkey.myRigidbody.linearVelocity = Vector2.zero;
                    _myMonkey._fsm.ChangeState(FSMStates.AttackState);
                }
            }
            else
            {
                _myMonkey.Move(-Mathf.Sign(_myMonkey.transform.position.x - (_character.transform.position.x + _myMonkey.attackDistance)));
                //Debug.LogError("Debug2: " + Vector2.Distance(new Vector2(_myMonkey.transform.position.x, 0), new Vector2((_character.transform.position.x + _myMonkey.attackDistance), 0)));
                if (Vector2.Distance(new Vector2(_myMonkey.transform.position.x, 0), new Vector2((_character.transform.position.x + _myMonkey.attackDistance), 0)) < 0.5f)
                {
                    _myMonkey.myRigidbody.linearVelocity = Vector2.zero;
                    _myMonkey._fsm.ChangeState(FSMStates.AttackState);
                }


            }
        }
    }


}
