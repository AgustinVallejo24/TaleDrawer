using UnityEngine;

public class MonkeySleepingState : BaseState
{
    Monkey _monkey;
    public MonkeySleepingState(Monkey monkey)
    {
        _monkey = monkey;
    }
    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _monkey.currentState = FSMStates.SleepingState;
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
