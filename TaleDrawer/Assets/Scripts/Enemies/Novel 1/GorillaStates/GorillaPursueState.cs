using UnityEngine;
using System.Linq;
public class GorillaPursueState : BaseState
{
    Character _character;
    Gorilla _myGorilla;
    bool isAttacking;
    CustomNode newNode;

    public GorillaPursueState(Gorilla gori, Character chara)
    {
        _character = chara;
        _myGorilla = gori;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myGorilla.currentState = FSMStates.PursuitState;
        _myGorilla.SetSpeed(10f);
        _myGorilla.ChangeAnimation(_myGorilla._moveT);
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
        if (Physics2D.OverlapBox(_myGorilla.transform.position, Vector2.one, 0, _myGorilla._targetMask) && !isAttacking)
        {
            isAttacking = true;
            newNode = _myGorilla.patrollingNodes.OrderBy(x => Vector3.Distance(_myGorilla.transform.position, x.transform.position)).First();

            if (_myGorilla._currentTarget.transform == _character.transform)
            {
                _character.SendInputToFSM(CharacterStates.Stop);
                _character.entityRigidbody.linearVelocityX = 0;
            }           

        }

        if (!isAttacking)
        {
            _myGorilla.Move(-Mathf.Sign(_myGorilla.transform.position.x - _myGorilla._currentTarget.position.x));
        }
        else
        {
            if ((Mathf.Sign(newNode.transform.position.x - _myGorilla.transform.position.x) > 0))
            {
                _myGorilla.Move(-Mathf.Sign(_myGorilla.transform.position.x - (_myGorilla._currentTarget.position.x - _myGorilla.attackDistance)));
                //Debug.LogError("Debug1: " + Vector2.Distance(new Vector2(_myMonkey.transform.position.x, 0), new Vector2((_character.transform.position.x - _myMonkey.attackDistance), 0)));
                if (Vector2.Distance(new Vector2(_myGorilla.transform.position.x, 0), new Vector2((_myGorilla._currentTarget.position.x - _myGorilla.attackDistance), 0)) < 0.5f && !_myGorilla.climbing)
                {
                    _myGorilla.entityRigidbody.linearVelocity = Vector2.zero;
                    _myGorilla._fsm.ChangeState(FSMStates.AttackState);
                }
            }
            else
            {
                _myGorilla.Move(-Mathf.Sign(_myGorilla.transform.position.x - (_myGorilla._currentTarget.position.x + _myGorilla.attackDistance)));
                //Debug.LogError("Debug2: " + Vector2.Distance(new Vector2(_myMonkey.transform.position.x, 0), new Vector2((_character.transform.position.x + _myMonkey.attackDistance), 0)));
                if (Vector2.Distance(new Vector2(_myGorilla.transform.position.x, 0), new Vector2((_myGorilla._currentTarget.position.x + _myGorilla.attackDistance), 0)) < 0.5f && !_myGorilla.climbing)
                {
                    _myGorilla.entityRigidbody.linearVelocity = Vector2.zero;
                    _myGorilla._fsm.ChangeState(FSMStates.AttackState);
                }


            }
        }
    }
}
