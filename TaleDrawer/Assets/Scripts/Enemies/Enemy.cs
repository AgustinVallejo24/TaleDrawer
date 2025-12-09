using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] protected int _health;
    [SerializeField] protected EnemyType _myType;
    [SerializeField] protected LayerMask _playerMask;
    public FSMStates currentState;

    [Header("Side scripts references")]
    public FSM _fsm;

    protected virtual void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void EnemyEvent()
    {

    }
    
    protected virtual void TakeDamage(int dmg, bool hasDeathState)
    {
        _health -= dmg;

        if( _health <= 0)
        {
            if (hasDeathState)
            {
                _fsm.ChangeState(FSMStates.DeathState);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }

    public virtual void CoroutineManager(IEnumerator cou, bool stop = false)
    {
        if (stop)
        {
            StopCoroutine(cou);
        }
        else
        {
            StartCoroutine(cou);
        }
            
    }
    
}

[System.Serializable]
public enum EnemyType
{
    EventEnemy,
    ActiveEnemy,
    MiniBoss,
    Boss
}