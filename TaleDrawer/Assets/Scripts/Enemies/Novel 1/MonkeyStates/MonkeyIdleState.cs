using System.Collections;
using UnityEngine;
using static Monkey;

public class MonkeyIdleState : BaseState
{
    Character _character;
    Monkey _myMonkey;
    IEnumerator _cou;
    
    float timer = 0;
    public MonkeyIdleState(Monkey monk, Character chara, IEnumerator cou)
    {
        _character = chara;
        _myMonkey = monk;
        _cou = cou;        
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myMonkey.currentState = FSMStates.IdleState;
        timer = 0;
        _myMonkey.ChangeAnimation(_myMonkey._idleT);
    }

    public override void OnExit()
    {
        _myMonkey.CoroutineManager(_cou, true);
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
        if(timer < 2)
        {
            timer += Time.deltaTime;

        }
        else
        {
            _myMonkey._fsm.ChangeState(FSMStates.PatrollState);
        }
    }    
}
