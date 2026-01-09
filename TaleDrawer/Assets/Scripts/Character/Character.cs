using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
public class Character : Entity
{
    #region Variables

    public float maxSpeed;
    public float currentSpeed;
    public float inAirSpeed;
    [SerializeField] [Range(0, 0.5f)] float _smoothSpeed;
    public float _maxLife;
    protected float _currentLife;
    public float jumpForce;

    [SerializeField] protected LayerMask _obstacleLayerMask;
    [SerializeField] protected LayerMask _floorLayerMask;
    [SerializeField] public Vector2 nextPosition;
    [SerializeField] bool _goToNextPosition;
    [SerializeField] public float yPositionOffset;
    [SerializeField] float _distToNodeThreshold;
    [SerializeField] Transform feetPosition;
    [SerializeField] protected Transform helmetPosition;

    [SerializeField] public List<CustomNode> _currentPath;
    public Action onMovingStart;
    public Action onMovingEnd;
    public IInteractable currentInteractable;
    public AudioSource characterAudioSource;
    public AudioClip jumpSound;

    #endregion

    #region References

    [SerializeField] public Rigidbody2D characterRigidbody;
    [SerializeField] protected Animator _animator;
    public CharacterStates _currentState;
    [SerializeField] protected SpriteRenderer _characterSprite;
    [SerializeField] LayerMask _walkableLayerMask;
    #endregion


    #region Side Scripts References

    CustomPathfinding _pathFinding;
    protected EventFSM<CharacterStates> _eventFSM;
    public CharacterModel characterModel;
    public CharacterView characterView;
    #endregion


    public Transform visual;
    private Vector3 lastPos;
    #region Level References
    public List<InterestPoint> pointList;
    public Queue<InterestPoint> currentObjectivesQueue = new Queue<InterestPoint>();
    public Queue<InterestPoint> mainObjectivesQueue;

    public Camera sceneCamera;
    [SerializeField] CustomNode _lookAtNode = null;
    [SerializeField] float _yDiffThreshold;

    public static Character instance;

    public Casco helmet;

    #endregion

    public bool test;
    [SerializeField] LayerMask _CliffMask;
    public Action climbAction;
    public Lever currentLever;

    public Hook currentHook;
    public Collider2D currentMovePosObj;

    public LayerMask floorLayerMask;
    public int flipSign;

    public float xOffset;
    public float yOffset;


    public Action currentJumpingAction;
    public float currentJumpingTime;
    public Vector2 currentJumpingPosition;

    public CinemachineFollow cameraFollow;

    public float input;
    public float horizontalJumpDir;

    public LayerMask playerExcludeLayer;

    public float landingVelocityThreshold;
    public bool grounded;

    public float distance;
    protected virtual void Awake()
    {

        foreach (var item in pointList)
        {
            currentObjectivesQueue.Enqueue(item);
        }

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
              .SetTransition(CharacterStates.Wait, Wait)
              .SetTransition(CharacterStates.Death, Death)
                .SetTransition(CharacterStates.Landing, Landing)
              .SetTransition(CharacterStates.Falling, Falling)
             .SetTransition(CharacterStates.EquippingHelmet, EquippingHelmet).Done();
        StateConfigurer.Create(Moving)
            .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Jumping, Jumping)
            .SetTransition(CharacterStates.Climb, Climb)
            .SetTransition(CharacterStates.DoingEvent, DoingEvent)
            .SetTransition(CharacterStates.Stop, Stop)
            .SetTransition(CharacterStates.Death, Death)
            .SetTransition(CharacterStates.Falling, Falling)
              .SetTransition(CharacterStates.Landing, Landing)
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
            .SetTransition(CharacterStates.Landing, Landing)
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
            .SetTransition(CharacterStates.Landing, Landing).Done();
        StateConfigurer.Create(Swaying)
            .SetTransition(CharacterStates.JumpingToRope, JumpingToRope).Done();
        StateConfigurer.Create(OnLadder)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(Death)
           .SetTransition(CharacterStates.Idle, Idle)
        .Done();
        StateConfigurer.Create(Falling)
           .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Landing, Landing)
        .Done();



        #endregion
        // _eventFSM.SendInput(CharacterStates.Idle);
        _eventFSM = new EventFSM<CharacterStates>(Idle);
        #region IDLE STATE

        Idle.OnEnter += x =>
        {
            _currentState = CharacterStates.Idle;
            characterView.OnIdle();
        };

        Idle.OnUpdate += () =>
        {
            if (input != 0)
            {
                SendInputToFSM(CharacterStates.Moving);
            }
            else
            {
                characterRigidbody.linearVelocityX = 0;
            }
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


        };
        Moving.OnFixedUpdate += () =>
        {

            if (input != 0)
            {
                characterModel.Move2(input);
            }
            else
            {
                characterRigidbody.linearVelocityX = 0;

                SendInputToFSM(CharacterStates.Idle);
            }


        };
        Moving.OnExit += x =>
        {
            _goToNextPosition = false;


        };

        #endregion


        #region FALLING STATE

        Falling.OnEnter += x =>
        {
            _currentState = CharacterStates.Falling;
            
            //  characterView.OnMove();

        };

        Falling.OnUpdate += () =>
        {


        };
        Falling.OnFixedUpdate += () =>
        {



        };
        Falling.OnExit += x =>
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


        #region LANDING STATE

        Landing.OnEnter += x =>
        {
            characterView.OnLand();

            characterRigidbody.linearVelocity = Vector2.zero;
            _currentState = CharacterStates.Landing;


        };

        Landing.OnUpdate += () =>
        {


        };
        Landing.OnFixedUpdate += () =>
        {

        };
        Landing.OnExit += x => { };

        #endregion

        #region JUMPING STATE

        Jumping.OnEnter += x =>
        {
            //     _characterRigidbody.linearVelocity = Vector2.zero;
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
            characterModel.Move2(input, .7f);
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

        };

        OnLadder.OnUpdate += () =>
        {


        };
        OnLadder.OnFixedUpdate += () =>
        {

        };

        OnLadder.OnExit += x =>
        {

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
        instance = this;
        _pathFinding = new CustomPathfinding(_obstacleLayerMask);
        characterRigidbody = GetComponent<Rigidbody2D>();
        yPositionOffset = Math.Abs(transform.position.y - feetPosition.position.y);

    //    StartCoroutine(SoilCheck());

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
        if (characterRigidbody.linearVelocityY > landingVelocityThreshold)
        {
            _animator.SetInteger("YVelocity", 1);
        }
        else if (characterRigidbody.linearVelocityY < -landingVelocityThreshold)
        {
            _animator.SetInteger("YVelocity", -1);
        }
        else
        {
            _animator.SetInteger("YVelocity", 0);
        }

        grounded = IsGrounded();

        _animator.SetBool("Grounded", grounded);
        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x,transform.position.y, Camera.main.transform.position.z), .1f);
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

    }
    public CustomNode GetLastPathNode()
    {
        if (_currentPath.Any())
        {
            return _currentPath.Last();
        }
        else
        {
            return null;
        }
    }
    public IEnumerator SendInputToFSM(CharacterStates newState, float time)
    {
        yield return new WaitForSeconds(time);
        _eventFSM.SendInput(newState);
    }
    public void PutOnHelmet(Casco helm, Rigidbody2D rb)
    {
        SendInputToFSM(CharacterStates.EquippingHelmet);
        helmet = helm;
        helmet.transform.rotation = helmet.neededRot;
        helmet.transform.position = helmetPosition.position;
        helmet.transform.parent = transform;
        Destroy(rb);

        if (_currentPath.Any())
        {
            SendInputToFSM(CharacterStates.Moving);
        }
        else if (Vector2.Distance(CustomTools.ToVector2(transform.position), nextPosition) > 1f)
        {
            _goToNextPosition = true;
            SendInputToFSM(CharacterStates.Moving);
        }
        else
        {
            SendInputToFSM(CharacterStates.Idle);
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Spikes")
        {
            Death();
        }
        //if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Robin_SFalling" && grounded)
        //{
        //    currentSpeed = _maxSpeed;
        //    characterView.OnLand();
        //}

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out SpawningObject spawningObject))
        {
            spawningObject.InteractionWithEntity();
        }
    }
    public void ClearPath()
    {

        _currentPath.Clear();
    }


    /// <summary>
    /// Para comparar con el primer nodo del path el value debe ser 0, para comparar con el último nodo del path el value debe ser 1, y para saber si el path contiene al node el value debe ser 2, 
    /// cualquier value diferente a los mencionados devuelve falso.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool ComparePathNodes(CustomNode node, int value, bool first = false)
    {
        if (value == 0)
        {
            /*if (!first)
            {
                first = true;
                Debug.LogError("Comparo con el primero, y devuelvo " + ComparePathNodes(node, value, first));
            } */
            return _currentPath.First().Equals(node);
        }
        else if (value == 1)
        {
            /*if (!first)
            {
                first = true;
                Debug.LogError("Comparo con el último, y devuelvo " + ComparePathNodes(node, value, first));
            }*/
            return _currentPath.Last().Equals(node);
        }
        else if (value == 2)
        {
            /*if (!first)
            {
                first = true;
                Debug.LogError("Comparo si esta dentro del path, y devuelvo " + ComparePathNodes(node, value, first));
            }*/
            return _currentPath.Contains(node);
        }
        else
        {
            Debug.LogError("No se compara el nodo de ninguna manera, por lo tanto se devuelve falso");
            return false;
        }
    }


    public bool GetPath(CustomNode goal, Vector2 nextPos)
    {
        nextPosition = nextPos;
        CustomNode start = CustomTools.GetClosestNode(transform.position, GameManager.instance.nodes);
        _currentPath = _pathFinding.AStar(start, goal);

        if (_currentPath.Count > 1 && _currentPath.SkipLast(1).Last().isClickable && Vector2.Distance(CustomTools.ToVector2(_currentPath.SkipLast(1).Last().transform.position), CustomTools.ToVector2(_currentPath.Last().transform.position))
            > Vector2.Distance(CustomTools.ToVector2(_currentPath.SkipLast(1).Last().transform.position), nextPosition)
            && _currentPath.SkipLast(1).Last().neighbours.Where(x => x.node == _currentPath.Last()).Last().nodeEvent.GetPersistentEventCount() == 0)
        {
            _currentPath = _currentPath.SkipLast(1).ToList();
            if (Physics2D.Raycast(goal.transform.position, (nextPos - CustomTools.ToVector2(goal.transform.position)).normalized, Vector2.Distance(CustomTools.ToVector2(goal.transform.position), nextPos), _obstacleLayerMask))
            {
                return false;
            }
        }
        if (_currentPath.Any())
        {
            return true;
        }
        else
        {
            Debug.LogError("NOHAYPATH");
            return false;
        }
    }
    public bool GetPath(CustomNode goal)
    {
        CustomNode start = CustomTools.GetClosestNode(transform.position, GameManager.instance.nodes);
        _currentPath = _pathFinding.AStar(start, goal);

        if (_currentPath.Any())
        {
            nextPosition = goal.transform.position;
            return true;
        }
        else
        {
            Debug.LogError("NOHAYPATH");
            return false;
        }
    }

    public List<CustomNode> GetPathList(CustomNode goal)
    {
        CustomNode start = CustomTools.GetClosestNode(transform.position, GameManager.instance.nodes);

        var list = _pathFinding.AStar(start, goal);

        if (list.Any())
        {
            Debug.LogError(goal.gameObject.name + " " + list.Count);
            return list;
        }
        else
        {
            return new List<CustomNode>();
        }
    }

    public void Land()
    {
        _eventFSM.SendInput(CharacterStates.Landing);

        if (_currentPath.Any())
        {
            StartCoroutine(SendInputToFSM(CharacterStates.Moving, 0.2f));
        }
        else
        {
            StartCoroutine(SendInputToFSM(CharacterStates.Idle, 0.2f));
        }

    }
    public override void LiftEntity()
    {
        SendInputToFSM(CharacterStates.Stop);
        characterRigidbody.gravityScale = 0;
    }
    public override void PutOnHelmet()
    {
        base.PutOnHelmet();
    }
    public override void ReleaseFromBalloon()
    {
        characterRigidbody.gravityScale = 3;
        SendInputToFSM(CharacterStates.Idle);
    }


    public List<CustomNode> GetCurrentPath()
    {
        return _currentPath;
    }
    public void Jump(Transform jumpPos)
    {
        Debug.Log("Llego al salto");
        Character.instance.SendInputToFSM(CharacterStates.Jumping);
        characterRigidbody.linearVelocity = Vector2.zero;

        characterModel.Jump(CustomTools.ToVector2(jumpPos.position), Land);
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(feetPosition.position, Vector2.one, 0, -transform.up, 0, playerExcludeLayer);
        //   Debug.DrawBox(feetPosition.position, feetPosition.position + Vector3.down * .5f);

        if (hit)
            Debug.LogError(hit.transform.gameObject.name);
        if (hit.collider == null || hit.collider.isTrigger)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public IEnumerator SoilCheck()
    {
       
        while (true)
        {
            yield return new WaitForSeconds(.1f);
            RaycastHit2D hit = Physics2D.BoxCast(feetPosition.position,Vector2.one*1f,0, -transform.up, distance, playerExcludeLayer);
         //   Debug.DrawBox(feetPosition.position, feetPosition.position + Vector3.down * .5f);
       
            if(hit)
             Debug.LogError(hit.transform.gameObject.name);
            if (hit.collider == null || hit.collider.isTrigger )
            {
                Debug.LogError("FALLLL");
                SendInputToFSM(CharacterStates.Falling);
            }


        }
    }

    void OnDrawGizmos()
    {
        if (!feetPosition) return;

        Vector2 size = Vector2.one * 1f;
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
    Landing = 1 << 5,
    OnRope = 1 << 6,
    JumpingToRope = 1 << 7,
    Climb = 1 << 8,
    EquippingHelmet = 1 << 9,
    DoingEvent = 1 << 10,
    Swaying = 1 << 11,
    OnLadder = 1 << 12,
    Death = 1 << 13,
    Falling = 1 << 14,
    All = ~0
}
