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

        if (_character.transform.position.x >= _myMonkey.transform.position.x)
        {
            _myMonkey.transform.DOMoveX(_character.transform.position.x - 1, 1).OnComplete(() => { Debug.LogError("GameOver"); });
        }
        else
        {
            _myMonkey.transform.DOMoveX(_character.transform.position.x + 1, 1).OnComplete(() => { Debug.LogError("GameOver"); });
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
        
    }

    
}
