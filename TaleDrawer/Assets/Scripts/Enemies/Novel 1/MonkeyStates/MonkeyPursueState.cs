using UnityEngine;

public class MonkeyPursueState : BaseState
{
    Character _character;
    Monkey _myMonkey;

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

    }

    public override void OnExit()
    {

    }

    public override void Transitions()
    {

    }

    public override void Update()
    {
        _myMonkey.Move(-Mathf.Sign(_myMonkey.transform.position.x - _character.transform.position.x));
        if (Physics2D.OverlapBox(_myMonkey.transform.position, Vector2.one, 0, _myMonkey._playerMask))
        {
            _myMonkey.myRigidbody.linearVelocity = Vector2.zero;

            _character.SendInputToFSM(CharacterStates.Stop);
            _myMonkey._fsm.ChangeState(FSMStates.AttackState);
        }
    }


}
