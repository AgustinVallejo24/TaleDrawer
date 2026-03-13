using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Monkey : Enemy
{
    [Header("References")]
    [SerializeField] bool _hasHelmet;
    [SerializeField] GameObject _helmet;
    
    [SerializeField] Vector3 _areaSize;
    [SerializeField] Character _character;
    [SerializeField] SpawnableObjectType _dangerousObjects;

    [Header("Animations")]
    //[SerializeField] Animator _myAnim;
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
    //public float stunnedTime;
    //public Transform _currentTarget { get;  private set; }
    //[SerializeField] string _currenTargetName;
    public Bait _currentBait;
    protected override void Awake()
    {
        _fsm = new FSM();

        _fsm.AddState(FSMStates.IdleState, new MonkeyIdleState(this, _character, StartBehaviour()));
        _fsm.AddState(FSMStates.PatrollState, new MonkeyPatrollingState(this, _character));
        _fsm.AddState(FSMStates.PursuitState, new MonkeyPursueState(this, _character));
        _fsm.AddState(FSMStates.AttackState, new MonkeyAttackEventState(this, _character));
        _fsm.AddState(FSMStates.DeathState, new MonkeyDeathState(this, _character));
        _fsm.AddState(FSMStates.StunnedState, new MonkeyStunnedState(this));
        _fsm.AddState(FSMStates.BalloonState, new MonkeyBalloonState(this));
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
  
    /*public void ChangeTarget(Transform target)
    {
        _currentTarget = target;
        _currenTargetName = target.gameObject.name;
        EnemyEvent();
    }*/    

    public override void HandleAggroRelease(Transform attacker)
    {
        base.HandleAggroRelease(attacker);

        _currentBait = null;
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

    /*public void StartStun(float time)
    {
        StartCoroutine(Stunned(time));
    }*/

    /*public IEnumerator Stunned(float time)
    {
        yield return new WaitForSeconds(time);
        _fsm.ChangeState(FSMStates.IdleState);
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Spikes")
        {
            _myAnim.SetTrigger("Explode");
            Destroy(gameObject,5f);
        }


    }    

    
    public override void Attack()
    {
        if(_currentBait != null)
        {
            _currentBait.attacker = this.transform;
            _currentBait.Delete();
        }
        else if(_currentTarget != null)
        {
            if (_character._hasHelmet)
            {
                _character.DestroyHelmet();
                _character.SendInputToFSM(CharacterStates.Idle);
                StopChasingTarget(FSMStates.StunnedState);
                //_fsm.ChangeState(FSMStates.StunnedState);
            }
            else
            {
                _character.Death();
            }
        }
        else
        {
            StopChasingTarget(FSMStates.IdleState);            
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
                var targets = Physics2D.OverlapAreaAll(transform.position - _areaSize, transform.position + _areaSize, _targetMask);

                var sortedtargets = targets.Select(t => new { Collider = t, bait = t.GetComponent<Bait>() })
                    .Where(x => x.bait == null || x.bait.aboveFloor)
                    .OrderByDescending(x => x.bait != null)                        
                    .ThenBy(x => Vector2.Distance(transform.position, x.Collider.transform.position))                        
                    .FirstOrDefault();

                if (sortedtargets != null)
                { 
                    if(sortedtargets.Collider.gameObject.TryGetComponent(out Bait bait))
                    {
                        _currentBait = bait;
                        _currentBait.AddEnemy(this);
                    }
                    ChangeTarget(sortedtargets.Collider.transform);
                }                
            }
            else if(currentState == FSMStates.SleepingState) 
            {

                if (Physics2D.OverlapArea(transform.position - _areaSize/2, transform.position + _areaSize / 2, _targetMask))
                {
                    ChangeAnimation("WakeUp");
                    _fsm.ChangeState(FSMStates.IdleState);
                }
            }

                yield return new WaitForSeconds(0.3f);
        }
    }    

}
