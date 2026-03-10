using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class Gorilla : Enemy
{
    [Header("Variables")]
    public float climbingSpeed;
    [SerializeField] Vector3 _areaSize;    
    [SerializeField] Vector3 _climbingAreaSize;
    [SerializeField] Vector3 _climbingAreaSize2;
    [SerializeField] Character _character;
    public CustomNode[] patrollingNodes;
    public CustomNode[] upperPatrollingNodes;
    public int currentNodeIndex;
    public int currentUpperNodeIndex;
    public float attackDistance;
    public bool climbing;
    public bool floorDetection;
    [SerializeField] float _attackJumpForce;
    //public float stunnedTime;
    bool _shortStun;
    public Bait _currentBait;

    [Header("Animations")]
    //[SerializeField] Animator _myAnim;
    public string _idleT { get; private set; } = "Idle";
    public string _moveT { get; private set; } = "Move";
    public string _moveT2 { get; private set; } = "Move2";
    public string _sleepT { get; private set; } = "Sleep";
    public string _wakeUpT { get; private set; } = "WakeUp";
    public string _attackT { get; private set; } = "Attack";
    public string _attackT2 { get; private set; } = "Attack2";
    public string _stunnedT { get; private set; } = "Stunned";
    public string _deathT { get; private set; } = "Death";
    public string _destroyBallon { get; private set; } = "DBaloon";

    


    protected override void Awake()
    {
        _fsm = new FSM();

        _fsm.AddState(FSMStates.IdleState, new GorillaIdleState(this));
        _fsm.AddState(FSMStates.PatrollState, new GorillaPatrollingState(this, _character));
        _fsm.AddState(FSMStates.PursuitState, new GorillaPursueState(this, _character));
        _fsm.AddState(FSMStates.AttackState, new GorillaAttackState(this, _character));
        _fsm.AddState(FSMStates.DeathState, new GorillaDeathState(this, _character));
        _fsm.AddState(FSMStates.StunnedState, new GorillaStunnedState(this));
        _fsm.AddState(FSMStates.BalloonState, new GorillaBalloonState(this));


        _fsm.ChangeState(FSMStates.PatrollState);
    }

    protected override void Start()
    {
        if (climbing) 
        {
            //transform.position = new Vector3(upperPatrollingNodes[0].transform.position.x, upperPatrollingNodes[0].transform.position.y - transform.localScale.y);
            transform.position = upperPatrollingNodes[0].transform.position;            
            entityRigidbody.gravityScale = 0;
        }
        

        StartCoroutine(StartBehaviour());
    }

    protected override void Update()
    {
        _fsm.Update();
    }

    private void FixedUpdate()
    {
        _fsm.FixedUpdate();
    }   

    public override void HandleAggroRelease(Transform attacker)
    {
        base.HandleAggroRelease(attacker);

        _currentBait = null;
    }

    protected override void EnemyEvent()
    {
        if ((climbing && _currentBait == null) || (climbing && currentState == FSMStates.PursuitState))
        {
            _fsm.ChangeState(FSMStates.AttackState);
        }
        else
        {
            _fsm.ChangeState(FSMStates.PursuitState);
        }
            
    }

    public void AttackFromAbove()
    {
        ChangeAnimation(_attackT2);
        entityRigidbody.linearVelocity = Vector3.zero;
        entityRigidbody.gravityScale = 0;
        entityRigidbody.AddForce(Vector2.down * _attackJumpForce, ForceMode2D.Impulse);
        CustomNode nearestNode = upperPatrollingNodes.OrderBy(x  => Vector2.Distance(transform.position, x.transform.position)).First();
        currentUpperNodeIndex = Array.IndexOf(upperPatrollingNodes, nearestNode);        
    }

    public void ShortStunActivationState(bool state)
    {
        _shortStun = state;        
    }
    

    public override void Attack()
    {
        if (_currentBait != null)
        {            
            _currentBait.Delete();
        }
        if (_character._hasHelmet)
        {
            _character.DestroyHelmet();            
        }
        _character.Death();
    }

    public override void StartStun(float time)
    {
        if (_shortStun)
        {
            StartCoroutine(Stunned(1f));
        }
        else
        {
            StartCoroutine(Stunned(time));
        }
            
    }

    /*public void ChangeAnimation(string anim)
    {
        if(_myAnim != null)
        StartCoroutine(ChangeAnim(anim));
    }
    public IEnumerator ChangeAnim(string anim)
    {
        _myAnim.SetTrigger(anim);
        yield return new WaitForSeconds(.3f);
        _myAnim.ResetTrigger(anim);
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.tag == "Spikes")
        {
            _myAnim.SetTrigger("Explode");
            Destroy(gameObject, 5f);
        }

        

        if (collision.gameObject.layer == 6 && floorDetection == true)
        {
            floorDetection = false;
            climbing = false;
            entityRigidbody.linearVelocity = Vector2.zero;
            _fsm.ChangeState(FSMStates.StunnedState);
        }

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _character.gameObject && floorDetection == true)
        {
            _character.Death();
        }
    }

    /*public IEnumerator Stunned(float time)
    {
        yield return new WaitForSeconds(time);
        _fsm.ChangeState(FSMStates.IdleState);
    }*/
    public IEnumerator StartBehaviour()
    {
        while (true)
        {
            if (currentState == FSMStates.PatrollState)
            {
                if (!climbing)
                {
                    var targets = Physics2D.OverlapAreaAll(transform.position - _areaSize, transform.position + _areaSize, _targetMask);

                    if (targets.Any())
                    {
                        var sortedtargets = targets.Select(t => new { Collider = t, bait = t.GetComponent<Bait>() })
                            .Where(x => x.bait == null || x.bait.aboveFloor)
                            .OrderByDescending(x => x.bait != null)
                            .ThenBy(x => Vector2.Distance(transform.position, x.Collider.transform.position))
                            .FirstOrDefault();

                        if (sortedtargets != null)
                        {
                            if (sortedtargets.Collider.gameObject.TryGetComponent(out Bait bait))
                            {
                                _currentBait = bait;
                                _currentBait.AddEnemy(this);
                            }
                            ChangeTarget(sortedtargets.Collider.transform);
                        }
                    }
                    
                }
                else
                {
                    var targets = Physics2D.OverlapAreaAll(transform.position - _climbingAreaSize, transform.position + new Vector3(_climbingAreaSize.x, transform.position.y), _targetMask);

                    if (targets.Any())
                    {
                        var sortedtargets = targets.Select(t => new { Collider = t, bait = t.GetComponent<Bait>() })                            
                            .OrderByDescending(x => x.bait != null)
                            .ThenBy(x => Vector2.Distance(transform.position, x.Collider.transform.position))
                            .FirstOrDefault();
                        if(sortedtargets != null)
                        {
                            if (sortedtargets.Collider.gameObject.TryGetComponent(out Character chara)
                            && _character.entityRigidbody.linearVelocity.x != 0f && Mathf.Sign(_character.entityRigidbody.linearVelocityX) != Mathf.Sign(entityRigidbody.linearVelocityX))
                            {
                                EnemyEvent();
                            }
                            else if(sortedtargets.Collider.gameObject.TryGetComponent(out Bait bait))
                            {
                                _currentBait = bait;
                                _currentBait.AddEnemy(this);
                                ChangeTarget(sortedtargets.Collider.transform);
                            }
                        }
                        
                    }                    
                    else
                    {
                        var targets2 = Physics2D.OverlapAreaAll(transform.position - _climbingAreaSize2, transform.position + new Vector3(_climbingAreaSize2.x, transform.position.y), _targetMask);

                        if(targets2 != null)
                        {
                            var sortedtargets = targets2.Select(t => new { Collider = t, bait = t.GetComponent<Bait>() })                                                            
                                .OrderByDescending(x => x.bait != null)                            
                                .ThenBy(x => Vector2.Distance(transform.position, x.Collider.transform.position))                            
                                .FirstOrDefault();

                            if(sortedtargets != null)
                            {
                                if (sortedtargets.Collider.gameObject.TryGetComponent(out Bait bait))
                                {
                                    _currentBait = bait;
                                    _currentBait.AddEnemy(this);
                                    ChangeTarget(sortedtargets.Collider.transform);
                                }
                                else
                                {
                                    EnemyEvent();
                                }
                                    
                            }
                        }

                        
                    }
                }


                
                /*if (Physics2D.OverlapArea(transform.position - _climbingAreaSize, transform.position + new Vector3(_climbingAreaSize.x, transform.position.y), _targetMask) && climbing && _character.entityRigidbody.linearVelocity.x != 0f
                     && Mathf.Sign(_character.entityRigidbody.linearVelocityX) != Mathf.Sign(entityRigidbody.linearVelocityX))
                {
                    EnemyEvent();
                }
                else if (Physics2D.OverlapArea(transform.position - _climbingAreaSize2, transform.position + new Vector3(_climbingAreaSize2.x, transform.position.y), _targetMask) && climbing)
                {
                    EnemyEvent();
                }*/
            }
            else if(currentState == FSMStates.PursuitState && _currentBait != null && climbing)
            {
                if (Physics2D.OverlapArea(transform.position - _climbingAreaSize2, transform.position + new Vector3(_climbingAreaSize2.x, transform.position.y), _targetMask))
                {
                    EnemyEvent();
                }
            }



                yield return new WaitForSeconds(0.3f);
        }
    }
}
