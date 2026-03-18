using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class Enemy : Entity
{
    [Header("Variables")]
    [SerializeField] protected int _health;
    [SerializeField] protected EnemyType _myType;
    [SerializeField] protected EnemyClass _myClass;
    [SerializeField] protected float _speed;
    public float stunnedTime;
    [SerializeField] protected Vector2 _movementVector;
    [SerializeField] public LayerMask _targetMask;
    public Transform _currentTarget { get; private set; }
    [SerializeField] string _currenTargetName;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public FSMStates currentState;
    [SerializeField] protected Animator _myAnim;

    [Header("Side scripts references")]
    public FSM _fsm;

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }
    public override void LiftEntity()
    {
        entityRigidbody.gravityScale = 0;
        _fsm.ChangeState(FSMStates.BalloonState);
    }

    public override void ReleaseFromBalloon()
    {
        entityRigidbody.gravityScale = 1;
        _fsm.ChangeState(FSMStates.IdleState);
    }

    public void Move(float x)
    {
        if (x != 0)
            Flip(CustomTools.ToVector2(transform.position) + new Vector2(x, 0));
        _movementVector = new Vector3(x, 0) * _speed;
        entityRigidbody.linearVelocityX = _movementVector.x;

    }

    public void Move(float x, float speed)
    {
        if (x != 0)
            Flip(CustomTools.ToVector2(transform.position) + new Vector2(x, 0));
        _movementVector = new Vector3(x, 0) * speed;
        entityRigidbody.linearVelocityX = _movementVector.x;

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

    public void ChangeAnimation(string anim)
    {
        if (_myAnim != null)
            StartCoroutine(ChangeAnim(anim));
    }
    public IEnumerator ChangeAnim(string anim)
    {
        _myAnim.SetTrigger(anim);
        yield return new WaitForSeconds(.3f);
        _myAnim.ResetTrigger(anim);
    }

    public virtual void Attack()
    {
        
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

    public virtual void StartStun(float time)
    {
        StartCoroutine(Stunned(time));
    }

    public IEnumerator Stunned(float time)
    {
        yield return new WaitForSeconds(time);
        _fsm.ChangeState(FSMStates.IdleState);
    }

    public virtual void ChangeTarget(Transform target)
    {
        _currentTarget = target;
        _currenTargetName = target.gameObject.name;
        EnemyEvent();
    }

    public virtual void StopChasingTarget(FSMStates nextState)
    {
        _fsm.ChangeState(nextState);
        _currentTarget = null;
        
    }

    public virtual void HandleAggroRelease(Transform attacker)
    {        
        if (this.transform == attacker)
        {
            StopChasingTarget(FSMStates.StunnedState);
        }
        else
        {
            StopChasingTarget(FSMStates.IdleState);
        }
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
        gameObject.SetActive(false);
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

[System.Serializable]
public enum EnemyClass
{
    Monkey,
    Gorilla
}