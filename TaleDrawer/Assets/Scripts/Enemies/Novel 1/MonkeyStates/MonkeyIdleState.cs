using System.Collections;
using UnityEngine;
using static Monkey;

public class MonkeyIdleState : BaseState
{
    Character _character;
    Monkey _myMonkey;
    IEnumerator _cou;


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
        
    }    
}
