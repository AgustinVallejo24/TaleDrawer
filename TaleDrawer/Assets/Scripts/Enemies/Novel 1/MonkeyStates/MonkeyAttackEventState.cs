using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using DG.Tweening;
public class MonkeyAttackEventState : BaseState
{
    Character _character;
    Monkey _myMonkey;

    public MonkeyAttackEventState(Monkey monk, Character chara)
    {
        _character = chara;
        _myMonkey = monk;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myMonkey.currentState = FSMStates.AttackState;
        _myMonkey.StartCoroutine(_myMonkey.Attack());

    }

    public override void OnExit()
    {
        
    }

    public override void Transitions()
    {
        
    }

    public override void Update()
    {
      

    }

    
    
}
