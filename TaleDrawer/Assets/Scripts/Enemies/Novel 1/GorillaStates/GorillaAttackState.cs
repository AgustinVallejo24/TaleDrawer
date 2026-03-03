using UnityEngine;

public class GorillaAttackState : BaseState
{
    Character _character;
    Gorilla _myGorilla;

    public GorillaAttackState(Gorilla gori, Character chara)
    {
        _myGorilla = gori;
        _character = chara;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _myGorilla.currentState = FSMStates.AttackState;
        _myGorilla.Flip(_character.transform.position);

        if (_myGorilla.climbing)
        {
            _myGorilla.AttackFromAbove();
        }
        else
        {
            
            _myGorilla.ChangeAnimation(_myGorilla._attackT);
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
