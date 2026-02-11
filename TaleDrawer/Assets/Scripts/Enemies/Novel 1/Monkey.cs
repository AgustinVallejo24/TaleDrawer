using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Monkey : Enemy
{
    [Header("References")]
    [SerializeField] bool _hasHelmet;
    [SerializeField] GameObject _helmet;
    
    [SerializeField] Vector3 _areaSize;
    [SerializeField] Character _character;
    [SerializeField] SpawnableObjectType _dangerousObjects;

    [Header("Animations")]
    [SerializeField] Animator _myAnim;
    public string _idleT { get; private set; } = "Idle"; 
    public string _moveT { get; private set; } = "Move";
    public string _sleepT { get; private set; } = "Sleep";
    public string _wakeUpT { get; private set; } = "WakeUp";
    public string _attackT { get; private set; } = "Attack";
    public string _deathT { get; private set; } = "Death";

    public CustomNode[] patrollingNodes;
    public int currentNodeIndex;
    public float attackDistance;
    public bool sleeping;
    protected override void Awake()
    {
        _fsm = new FSM();

        _fsm.AddState(FSMStates.IdleState, new MonkeyIdleState(this, _character, StartBehaviour()));
        _fsm.AddState(FSMStates.PatrollState, new MonkeyPatrollingState(this, _character));
        _fsm.AddState(FSMStates.PursuitState, new MonkeyPursueState(this, _character));
        _fsm.AddState(FSMStates.AttackState, new MonkeyAttackEventState(this, _character));
        _fsm.AddState(FSMStates.DeathState, new MonkeyDeathState(this, _character));
        _fsm.AddState(FSMStates.StunnedState, new MonkeyStunnedState(this));
        _fsm.AddState(FSMStates.SleepingState, new MonkeySleepingState(this));
        if (sleeping)
        {
            _fsm.ChangeState(FSMStates.SleepingState);
            ChangeAnimation("Sleep");
        }
        else
        {
            _fsm.ChangeState(FSMStates.PatrollState);
        }
       
    }
    protected override void Start()
    {        
        if( _hasHelmet)
        {
            _helmet.SetActive(true);
        }
        StartCoroutine(StartBehaviour());

        if (sleeping)
        {
            ChangeAnimation("Sleep");
        }
    }
  


    protected override void EnemyEvent()
    {
        _fsm.ChangeState(FSMStates.PursuitState);
    }

    protected override void Update()
    {
        _fsm.Update();
    }

    private void FixedUpdate()
    {
        _fsm.FixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out SpawningObject obj) && _dangerousObjects.HasFlag(obj.myType))
        {
            Destroy(obj.gameObject);

            if (_hasHelmet)
            {
                Debug.Log("No me hace nada");
            }
            else
            {
                TakeDamage(2, true);
                
            }
                
        }
        if (collision.gameObject.tag == "Spikes")
        {
            Destroy(gameObject);
        }


    }    
    public void ChangeAnimation(string anim)
    {
        StartCoroutine(ChangeAnim(anim));
    }
    public IEnumerator ChangeAnim(string anim)
    {
        _myAnim.SetTrigger(anim);
        yield return new WaitForSeconds(.3f);
        _myAnim.ResetTrigger(anim);
    }
    
    public void Attack()
    {
        if (_character._hasHelmet)
        {
            _character.DestroyHelmet();
            _character.SendInputToFSM(CharacterStates.Idle);
            _fsm.ChangeState(FSMStates.StunnedState);
        }
        else
        {
            _character.Death();
        }
            
    }
    /*public IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);
        
    }*/
    public IEnumerator StartBehaviour()
    {
        while (true)
        {
            if(currentState == FSMStates.IdleState || currentState == FSMStates.PatrollState)
            {

                if (Physics2D.OverlapArea(transform.position - _areaSize, transform.position + _areaSize, _playerMask))
                {
                    Debug.Log("No me hace nada");
                    EnemyEvent();
                }
            }
            else if(currentState == FSMStates.SleepingState) 
            {

                if (Physics2D.OverlapArea(transform.position - _areaSize/2, transform.position + _areaSize / 2, _playerMask))
                {
                    ChangeAnimation("WakeUp");
                    _fsm.ChangeState(FSMStates.IdleState);
                }
            }


            yield return new WaitForSeconds(0.3f);
        }
    }    

}
