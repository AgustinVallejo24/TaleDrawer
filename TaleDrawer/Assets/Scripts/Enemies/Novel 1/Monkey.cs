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


    protected override void Awake()
    {
        _fsm = new FSM();

        _fsm.AddState(FSMStates.IdleState, new MonkeyIdleState(this, _character));
        _fsm.AddState(FSMStates.AttackState, new MonkeyAttackEventState(this, _character));
        _fsm.AddState(FSMStates.DeathState, new MonkeyDeathState(this, _character));
        _fsm.ChangeState(FSMStates.IdleState);
    }
    protected override void Start()
    {        
        if( _hasHelmet)
        {
            _helmet.SetActive(true);
        }        
    }

    protected override void EnemyEvent()
    {
        _fsm.ChangeState(FSMStates.AttackState);
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
        
    }

    

    public IEnumerator StartBehaviour()
    {
        while (true)
        {
            if (Physics2D.OverlapArea(transform.position - _areaSize, transform.position + _areaSize, _playerMask))
            {
                EnemyEvent();
            }

            yield return new WaitForSeconds(0.3f);
        }
    }    

}
