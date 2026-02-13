using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
public class Character : Entity
{
    #region Variables

    public float maxSpeed;
    public float currentSpeed;
    public float inAirSpeed;
    public float climbingSpeed;
    public float climbingSpeedMultiplier;
    [SerializeField] [Range(0, 0.5f)] float _smoothSpeed;
    public float _maxLife;
    protected float _currentLife;
    public float jumpForce;
    float _originalGravityScale;
    [SerializeField] protected LayerMask _obstacleLayerMask;
    [SerializeField] protected LayerMask _floorLayerMask;
    [SerializeField] Transform feetPosition;
    [SerializeField] protected Transform helmetPosition;

    public IInteractableP currentInteractable;
    public IInteractableP currentInteraction;
    public AudioSource characterAudioSource;
    public AudioClip jumpSound;

    public Vector3 maxClimbingPos;
    public Vector3 minClimbingPos;
    public bool _hasHelmet;
    
    #endregion

    #region References

    [SerializeField] public Rigidbody2D characterRigidbody;
    [SerializeField] protected Animator _animator;
    public CharacterStates _currentState;
    public CharacterStates _climbingTransitions;
    [SerializeField] protected SpriteRenderer _characterSprite;
    [SerializeField] public SpriteRenderer keySprite;
    [SerializeField] LayerMask _walkableLayerMask;
    public Collider2D _mainCollider;
    public SpawningObject helmet;

   
    #endregion


    #region Side Scripts References

    protected EventFSM<CharacterStates> _eventFSM;
    public CharacterModel characterModel;
    public CharacterView characterView;
    #endregion


    public Transform visual;

    #region Level References

    public Camera sceneCamera;
    public static Character instance;
    

    #endregion

    public bool test;
    [SerializeField] LayerMask _CliffMask;
    public Action climbAction;
    public Activator currentActivator;

    public Hook currentHook;

    public LayerMask floorLayerMask;
    public LayerMask cliffMask;
    public int flipSign;

    public CinemachineFollow cameraFollow;
    public CinemachineGroupFraming groupFraming;
    public float xInput;
    public Vector2 climbingInputs;
    public float horizontalJumpDir;

    public LayerMask playerExcludeLayer;

    public float landingVelocityThreshold;
    public bool grounded;

    public float distance;

    public float cliffDetectionDistance;

    public List<AudioClip> stepClips;



    protected virtual void Awake()
    {
        instance = this;

        #region States Declarations
        var Idle = new StateE<CharacterStates>("Idle");
        var Moving = new StateE<CharacterStates>("Moving");
        var Wait = new StateE<CharacterStates>("Wait");
        var Jumping = new StateE<CharacterStates>("Jumping");
        var Stop = new StateE<CharacterStates>("Stop");
        var Landing = new StateE<CharacterStates>("Landing");
        var OnRope = new StateE<CharacterStates>("OnRope");
        var JumpingToRope = new StateE<CharacterStates>("JumpingToRope");
        var Climb = new StateE<CharacterStates>("Climb");
        var EquippingHelmet = new StateE<CharacterStates>("EquippingHelmet");
        var DoingEvent = new StateE<CharacterStates>("DoingEvent");
        var Swaying = new StateE<CharacterStates>("Swaying");
        var OnLadder = new StateE<CharacterStates>("OnLadder");
        var Death = new StateE<CharacterStates>("Death");
        var Falling = new StateE<CharacterStates>("Falling");
        StateConfigurer.Create(Idle)
            .SetTransition(CharacterStates.Moving, Moving)
             .SetTransition(CharacterStates.Jumping, Jumping)
             .SetTransition(CharacterStates.Climb, Climb)
             .SetTransition(CharacterStates.DoingEvent, DoingEvent)
             .SetTransition(CharacterStates.Stop, Stop)
             .SetTransition(CharacterStates.OnLadder, OnLadder)
             .SetTransition(CharacterStates.Wait, Wait)
             .SetTransition(CharacterStates.Death, Death)
             .SetTransition(CharacterStates.EquippingHelmet, EquippingHelmet).Done();
        StateConfigurer.Create(Moving)
            .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Jumping, Jumping)
            .SetTransition(CharacterStates.OnLadder, OnLadder)
            .SetTransition(CharacterStates.Climb, Climb)
            .SetTransition(CharacterStates.DoingEvent, DoingEvent)
            .SetTransition(CharacterStates.Stop, Stop)
            .SetTransition(CharacterStates.Death, Death)
            .SetTransition(CharacterStates.Swaying, Swaying)
            .SetTransition(CharacterStates.EquippingHelmet, EquippingHelmet).Done();
        StateConfigurer.Create(Wait)
            .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Jumping, Jumping)
            .SetTransition(CharacterStates.OnRope, OnRope)
            .SetTransition(CharacterStates.DoingEvent, DoingEvent)
            .SetTransition(CharacterStates.Stop, Stop)
                .SetTransition(CharacterStates.Death, Death)
            .SetTransition(CharacterStates.EquippingHelmet, EquippingHelmet)
            .SetTransition(CharacterStates.OnLadder, OnLadder).Done();
        StateConfigurer.Create(Jumping)
            .SetTransition(CharacterStates.Idle, Idle)
             .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Climb, Climb)
            .Done();
        StateConfigurer.Create(Stop)
            .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Moving, Moving)
             .SetTransition(CharacterStates.Death, Death)
            .SetTransition(CharacterStates.Wait, Wait).Done();
        StateConfigurer.Create(Landing)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Stop, Stop)
            .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(OnRope)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Stop, Stop)
            .SetTransition(CharacterStates.JumpingToRope, JumpingToRope)
            .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(Climb)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Moving, Moving)
           .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(EquippingHelmet)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Moving, Moving)
           .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(DoingEvent)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(JumpingToRope)
            .SetTransition(CharacterStates.Swaying, Swaying)
            .SetTransition(CharacterStates.Moving, Moving).Done();
        StateConfigurer.Create(Swaying)
            .SetTransition(CharacterStates.JumpingToRope, JumpingToRope).Done();

        StateConfigurer.Create(OnLadder)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Jumping, Jumping)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Climb, Climb)
            .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(Death)
           .SetTransition(CharacterStates.Idle, Idle)
        .Done();
        StateConfigurer.Create(Falling)
           .SetTransition(CharacterStates.Idle, Idle)
           .Done();



        #endregion


        _eventFSM = new EventFSM<CharacterStates>(Idle); 


        #region IDLE STATE

        Idle.OnEnter += x =>
        {
            _currentState = CharacterStates.Idle;
            characterView.OnIdle();
        };

        Idle.OnUpdate += () =>
        {
            if (xInput != 0)
            {
                SendInputToFSM(CharacterStates.Moving);
            }
            else
            {
                characterRigidbody.linearVelocityX = 0;
            }

            characterModel.AlignToGround();
        };

        Idle.OnExit += x => { };

        #endregion

        #region MOVING STATE

        Moving.OnEnter += x =>
        {
            _currentState = CharacterStates.Moving;
            characterView.OnMove();

        };

        Moving.OnUpdate += () =>
        {

            characterModel.AlignToGround();
        };
        Moving.OnFixedUpdate += () =>
        {

            if (xInput != 0)
            {
                characterModel.Move2(xInput);
            }
            else
            {
                characterRigidbody.linearVelocityX = 0;

                SendInputToFSM(CharacterStates.Idle);
            }


        };
        Moving.OnExit += x =>
        {
         


        };

        #endregion

        #region WAIT STATE

        Wait.OnEnter += x =>
        {
            characterView.OnIdle();
            Debug.Log("Entro aca");
            characterRigidbody.linearVelocity = Vector2.zero;
            _currentState = CharacterStates.Wait;


        };

        Wait.OnUpdate += () =>
        {


        };
        Wait.OnFixedUpdate += () =>
        {

        };
        Wait.OnExit += x => { };

        #endregion

        #region JUMPING STATE

        Jumping.OnEnter += x =>
        {
            horizontalJumpDir = characterRigidbody.linearVelocity.x;
            characterRigidbody.linearVelocity = Vector2.zero;
            _currentState = CharacterStates.Jumping;
            characterView.OnJump();
            characterModel.Jump();
        };

        Jumping.OnUpdate += () =>
        {


        };
        Jumping.OnFixedUpdate += () =>
        {
        };
        Jumping.OnExit += x => { };

        #endregion

        #region ONROPE STATE

        OnRope.OnEnter += x =>
        {

            _currentState = CharacterStates.OnRope;


        };

        OnRope.OnUpdate += () =>
        {


        };
        OnRope.OnFixedUpdate += () =>
        {

        };
        OnRope.OnExit += x => { };

        #endregion

        #region JUMPINGTOROPE STATE

        JumpingToRope.OnEnter += x =>
        {
            _currentState = CharacterStates.JumpingToRope;
        };

        JumpingToRope.OnUpdate += () =>
        {

            if (grounded)
            {
                SendInputToFSM(CharacterStates.Moving);
            }
        };
        JumpingToRope.OnFixedUpdate += () =>
        {

        };

        JumpingToRope.OnExit += x =>
        {

        };

        #endregion

        #region SWAYING STATE

        Swaying.OnEnter += x =>
        {


            _currentState = CharacterStates.Swaying;
        };

        Swaying.OnUpdate += () =>
        {


        };
        Swaying.OnFixedUpdate += () =>
        {

        };

        Swaying.OnExit += x =>
        {

        };

        #endregion

        #region ONLADDER STATE

        OnLadder.OnEnter += x =>
        {            
            _currentState = CharacterStates.OnLadder;
            characterRigidbody.gravityScale = 0;
            _mainCollider.isTrigger = true;
            if (currentHook != null)
            {
                currentHook.RopeAnimationManager(1);
            }

        };

        OnLadder.OnUpdate += () =>
        {
            _animator.speed = Mathf.Clamp(Mathf.Abs(climbingInputs.y),0f,1);


        };
        OnLadder.OnFixedUpdate += () =>
        {
            if(climbingInputs.y != 0)
            {
                characterModel.Climb(climbingInputs.y, maxClimbingPos.y, minClimbingPos.y, climbingSpeedMultiplier);
            }
            else
            {
                characterRigidbody.linearVelocityY = 0f;
            }
            
        };

        OnLadder.OnExit += x =>
        {
            characterView.OnExitLadder();
            _animator.speed = 1;
            if (currentInteraction != null)
            {
                currentInteraction.OnLeavingInteraction();
            }
            if(currentHook!= null)
            {
                currentHook.RopeAnimationManager(0);                
            }           
            currentInteractable = null;
            currentHook = null;
            characterRigidbody.gravityScale = _originalGravityScale;
            _mainCollider.isTrigger = false;
            maxClimbingPos = Vector3.positiveInfinity;
            maxClimbingPos = Vector3.negativeInfinity;

            if (!grounded)
            {
                characterRigidbody.linearVelocityX = climbingInputs.x;
            }            
            climbingInputs = Vector2.zero;
            
        };

        #endregion

        #region STOP STATE

        Stop.OnEnter += x =>
        {

            _currentState = CharacterStates.Stop;
            characterView.OnIdle();

        };

        Stop.OnUpdate += () =>
        {

        };
        Stop.OnFixedUpdate += () =>
        {

        };
        Stop.OnExit += x => { };

        #endregion


        #region DEATH STATE

        Death.OnEnter += x =>
        {

            _currentState = CharacterStates.Death;
            characterView.OnDeath();
            StartCoroutine(DeathCoroutine());
        };

        Death.OnUpdate += () =>
        {


        };
        Death.OnFixedUpdate += () =>
        {

        };
        Death.OnExit += x => { };

        #endregion

        #region CLIMB STATE

        Climb.OnEnter += x =>
        {
            Debug.LogError("CLIMBEO");
            characterRigidbody.gravityScale = 0;
            characterRigidbody.linearVelocity = Vector2.zero;
            _currentState = CharacterStates.Climb;
            characterView.OnClimb();

        };

        Climb.OnUpdate += () =>
        {

        };
        Climb.OnFixedUpdate += () =>
        {

        };
        Climb.OnExit += x => { };

        #endregion

        #region EQUIPPING HELMET STATE

        EquippingHelmet.OnEnter += x =>
        {
            _currentState = CharacterStates.EquippingHelmet;
            characterView.OnEquippingHelmet();

        };

        EquippingHelmet.OnUpdate += () =>
        {

        };
        EquippingHelmet.OnFixedUpdate += () =>
        {

        };
        EquippingHelmet.OnExit += x => { };

        #endregion

        #region DOING EVENT

        DoingEvent.OnEnter += x =>
        {
            _currentState = CharacterStates.DoingEvent;

        };

        DoingEvent.OnUpdate += () =>
        {

        };
        DoingEvent.OnFixedUpdate += () =>
        {

        };
        DoingEvent.OnExit += x => { };

        #endregion
        _eventFSM.EnterFirstState();


    }



    protected virtual void Start()
    {
      
        characterRigidbody = GetComponent<Rigidbody2D>();
        _originalGravityScale = characterRigidbody.gravityScale;
    }
    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ClimbCliff()
    {
        SendInputToFSM(CharacterStates.Climb);
    }
    public virtual void Update()
    {
        _eventFSM.Update();
        if(Mathf.Abs(characterRigidbody.linearVelocityX) > 0)
        {
            _animator.SetInteger("XVelocity", 1);
        }
        else
        {
            _animator.SetInteger("XVelocity", 0);
        }
        if (characterRigidbody.linearVelocityY > landingVelocityThreshold && _currentState != CharacterStates.OnLadder)
        {
            _animator.SetInteger("YVelocity", 1);
        }
        else if (characterRigidbody.linearVelocityY < -landingVelocityThreshold && _currentState != CharacterStates.OnLadder)
        {
            _animator.SetInteger("YVelocity", -1);
        }
        else if(_currentState != CharacterStates.OnLadder)
        {
            _animator.SetInteger("YVelocity", 0);
        }

        grounded = IsGrounded();

        _animator.SetBool("Grounded", grounded);

        if (CheckCliff())
        {
            SendInputToFSM(CharacterStates.Climb);
        }
     
    }
    private void FixedUpdate()
    {
        _eventFSM.FixedUpdate();
    }
    public void SendInputToFSM(CharacterStates newState)
    {

        _eventFSM.SendInput(newState);
    }
    public override void Death()
    {
        base.Death();
        SendInputToFSM(CharacterStates.Death);
        characterRigidbody.linearVelocity = Vector2.zero;
        if (currentBalloon != null)
        {
            currentBalloon.Delete();
        }

    }

    public float GetAnimatorSpeed()
    {
        return _animator.speed;
    }
    public IEnumerator SendInputToFSM(CharacterStates newState, float time)
    {
        yield return new WaitForSeconds(time);
        _eventFSM.SendInput(newState);
    }

    public override void PutOnHelmet()
    {
        _hasHelmet = true;
        GameManager.instance.AddSpawningObject(helmet);
        helmet.gameObject.SetActive(true);
        helmet.GetComponent<Casco>().InstantPaint();
    }

    public void DestroyHelmet()
    {
        _hasHelmet = false;
        helmet.Delete();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
    
        if (collision.gameObject.tag == "Spikes")
        {
            Death();
        }
        if(collision.TryGetComponent(out IInteractableP interactable))
        {
            currentInteractable = interactable;
            ShowKeyUI(currentInteractable.InteractionKey());
        }

    }


    public virtual void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out IInteractableP interactable) && interactable == currentInteractable)
        {
            HideKeyUI();
            currentInteractable = null;
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out SpawningObject spawningObject))
        {
            spawningObject.InteractionWithEntity();
        }
    }


    public override void LiftEntity()
    {
        
        SendInputToFSM(CharacterStates.Stop);
        characterRigidbody.gravityScale = 0;
    }
    
    public override void ReleaseFromBalloon()
    {
        currentBalloon = null;
        characterRigidbody.gravityScale = 3;
        SendInputToFSM(CharacterStates.Idle);
    }

    public bool CheckCliff()
    {

        if (Physics2D.Raycast(transform.position + Vector3.up *.5f, transform.right * flipSign, cliffDetectionDistance, cliffMask) && !Physics2D.Raycast(transform.position + 0.75f* Vector3.up, transform.right * flipSign, cliffDetectionDistance, cliffMask))
        {
            if (Mathf.Abs(xInput) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(feetPosition.position, Vector2.one*.8f, 0, -transform.up, 0, playerExcludeLayer);

        if (hit.collider == null || hit.collider.isTrigger)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ShowKeyUI(KeyCode key)
    {
        keySprite.sprite = Tutorial.instance.keysSprites[key];
    }

    public void HideKeyUI()
    {
        keySprite.sprite = null;
    }
    void OnDrawGizmos()
    {

        // Definimos la dirección y distancia para no repetir código
        Vector2 direction1 = transform.right * flipSign;
        float distance1 = cliffDetectionDistance;

        // 1. Rayo Inferior (desde la posición del objeto)
        Debug.DrawRay(transform.position + Vector3.up*.5f, direction1 * distance1, Color.white);

        // 2. Rayo Superior (desde la posición + 1 unidad hacia arriba)
        Debug.DrawRay(transform.position + Vector3.up, direction1 * distance1, Color.white);
        if (!feetPosition) return;

        Vector2 size = Vector2.one * .8f;
        Vector3 direction = -transform.up; // La dirección del cast
        Vector3 startPosition = feetPosition.position;
        Vector3 endPosition = startPosition + (direction * distance);

        // Color para el Gizmo
        Gizmos.color = Color.green;

        // 1. Dibujar la caja inicial
        Gizmos.DrawWireCube(startPosition, size);

        // 2. Dibujar la caja final
        Gizmos.DrawWireCube(endPosition, size);

        // 3. Dibujar líneas que conecten las esquinas (opcional pero muy útil)
        Gizmos.DrawLine(startPosition, endPosition);
    }
    public void SetAnimatorTrigger(string name)
    {
        _animator.SetTrigger(name);
    }
}

[System.Serializable]
[Flags]
public enum CharacterStates
{
    None = 0,
    Idle = 1 << 0,
    Moving = 1 << 1,
    Wait = 1 << 2,
    Stop = 1 << 3,
    Jumping = 1 << 4,
    OnRope = 1 << 5,
    JumpingToRope = 1 << 6,
    Climb = 1 << 7,
    EquippingHelmet = 1 << 8,
    DoingEvent = 1 << 9,
    Swaying = 1 << 10,
    OnLadder = 1 << 11,
    Death = 1 << 12,
    All = ~0
}
