using System.Collections;
using UnityEngine;

public abstract class Enemy : Entity
{
    [Header("Variables")]
    [SerializeField] protected int _health;
    [SerializeField] protected EnemyType _myType;
    [SerializeField] protected float _speed;
    [SerializeField] protected Vector2 _movementVector;
    [SerializeField] public LayerMask _playerMask;
    public Rigidbody2D myRigidbody;
    public SpriteRenderer spriteRenderer;
    public FSMStates currentState;


    [Header("Side scripts references")]
    public FSM _fsm;

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }
    public override void LiftEntity()
    {
        myRigidbody.gravityScale = 0;
        _fsm.ChangeState(FSMStates.BalloonState);
    }

    public override void ReleaseFromBalloon()
    {
        myRigidbody.gravityScale = 1;
        _fsm.ChangeState(FSMStates.IdleState);
    }

    public void Move(float x)
    {
        if (x != 0)
            Flip(CustomTools.ToVector2(transform.position) + new Vector2(x, 0));
        _movementVector = new Vector3(x, 0) * _speed;
        myRigidbody.linearVelocityX = _movementVector.x;

    }
    public void Flip(Vector3 position)
    {
        if (Mathf.Sign(position.x - transform.position.x) > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
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
                DeathEvent();
            }
            
        }
    }

    public virtual void DeathEvent()
    {
        Destroy(gameObject);
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