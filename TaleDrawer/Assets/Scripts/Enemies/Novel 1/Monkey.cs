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

    public CustomNode[] patrollingNodes;
    public int currentNodeIndex;
    protected override void Awake()
    {
        _fsm = new FSM();

        _fsm.AddState(FSMStates.IdleState, new MonkeyIdleState(this, _character, StartBehaviour()));
        _fsm.AddState(FSMStates.PatrollState, new MonkeyPatrollingState(this, _character));
        _fsm.AddState(FSMStates.AttackState, new MonkeyAttackEventState(this, _character));
        _fsm.AddState(FSMStates.DeathState, new MonkeyDeathState(this, _character));
        _fsm.AddState(FSMStates.StunnedState, new MonkeyStunnedState());
        _fsm.ChangeState(FSMStates.PatrollState);
    }
    protected override void Start()
    {        
        if( _hasHelmet)
        {
            _helmet.SetActive(true);
        }
        StartCoroutine(StartBehaviour());
    }
  


    protected override void EnemyEvent()
    {
        _fsm.ChangeState(FSMStates.AttackState);
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


            yield return new WaitForSeconds(0.3f);
        }
    }    

}
