using UnityEngine;
using static Monkey;

public class MonkeyIdleState : BaseState
{
    Character _character;
    Monkey _myMonkey;

    public MonkeyIdleState(Monkey monk, Character chara)
    {
        _character = chara;
        _myMonkey = monk;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myMonkey.currentState = FSMStates.IdleState;
        _myMonkey.CoroutineManager(_myMonkey.StartBehaviour());
    }

    public override void OnExit()
    {
        _myMonkey.CoroutineManager(_myMonkey.StartBehaviour(), true);
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
        
    }    
}
