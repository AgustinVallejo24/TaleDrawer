using UnityEngine;

public class MonkeyStunnedState : BaseState
{
    Monkey _monkey;
    public override void FixedUpdate()
    {
      
    }

    public MonkeyStunnedState(Monkey monkey)
    {
        _monkey = monkey;
    }
    public override void OnEnter()
    {
        _monkey.currentState = FSMStates.StunnedState;
        _monkey.ChangeAnimation("Balloon");
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
