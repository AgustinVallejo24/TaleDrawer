using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;

public class Character : MonoBehaviour
{
    #region Variables

    [SerializeField] float _maxSpeed;
    public float currentSpeed;
    [SerializeField][Range(0, 0.5f)] float _smoothSpeed;
    [SerializeField] float _maxLife;
    protected float _currentLife;
    [SerializeField] float _jumpForce;
    public Vector2 currentJumpForce;
    [SerializeField] protected LayerMask _obstacleLayerMask;
    [SerializeField] protected LayerMask _floorLayerMask;
    [SerializeField] public Vector2 nextPosition;
    [SerializeField] public float yPositionOffset;
    [SerializeField] Transform feetPosition;
    #endregion

    #region References

    [SerializeField] protected Rigidbody2D _characterRigidbody;
    [SerializeField] protected Animator _animator;
    public CharacterStates _currentState;
    [SerializeField] protected SpriteRenderer _characterSprite;
    
    #endregion


    #region Side Scripts References

    PathFindingThetaStar _pathFinding;
    protected EventFSM<CharacterStates> _eventFSM;
    public CharacterModel characterModel;
    public CharacterView characterView;
    #endregion



    #region Level References
    public List<InterestPoint> pointList;
    public Queue<InterestPoint> currentObjectivesQueue = new Queue<InterestPoint>();
    public Queue<InterestPoint> mainObjectivesQueue;

    public Camera sceneCamera;

    public static Character instance;

    #endregion

    

    protected virtual void Awake()
    {
     
        foreach (var item in pointList)
        {
            currentObjectivesQueue.Enqueue(item);
        }
        var Idle = new StateE<CharacterStates>("Idle");
        var Moving = new StateE<CharacterStates>("Moving");
        var Wait = new StateE<CharacterStates>("Wait");
        var Jumping = new StateE<CharacterStates>("Jumping");
        var Stop = new StateE<CharacterStates>("Stop");
        StateConfigurer.Create(Idle).SetTransition(CharacterStates.Moving, Moving).SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(Moving).SetTransition(CharacterStates.Idle, Idle).SetTransition(CharacterStates.Wait, Wait).SetTransition(CharacterStates.Moving, Moving).Done();
        StateConfigurer.Create(Wait).SetTransition(CharacterStates.Idle, Idle).SetTransition(CharacterStates.Moving, Moving).SetTransition(CharacterStates.Jumping, Jumping).Done();
        StateConfigurer.Create(Jumping).SetTransition(CharacterStates.Idle, Idle).SetTransition(CharacterStates.Moving, Moving).SetTransition(CharacterStates.Stop, Stop).Done();
        StateConfigurer.Create(Stop).SetTransition(CharacterStates.Idle, Idle).SetTransition(CharacterStates.Moving, Moving).SetTransition(CharacterStates.Wait, Stop).Done();
        // _eventFSM.SendInput(CharacterStates.Idle);
        _eventFSM = new EventFSM<CharacterStates>(Idle);
        #region IDLE STATE

        Idle.OnEnter += x =>
        {
            
            _currentState = CharacterStates.Idle;
            characterView.OnIdle();
            /*if (currentObjectivesQueue.Any())
            {
                _eventFSM.SendInput(CharacterStates.Moving);
            }
            else
            {
               
            }*/
        };

        Idle.OnUpdate += () => 
        {
          
        };

        Idle.OnExit += x => { };

        #endregion

        #region MOVING STATE

        Moving.OnEnter += x =>
        {
            _currentState = CharacterStates.Moving;
            characterView.OnMove();
            /*if (currentObjectivesQueue.Any())
            {
                characterView.OnMove();
            }
            else
            {
                _eventFSM.SendInput(CharacterStates.Wait);
            }*/
            
        };

        Moving.OnUpdate += () => 
        {
            

        };
        Moving.OnFixedUpdate += () =>
        {
            /*if (Vector2.Distance(currentObjectivesQueue.Peek().transform.position, transform.position) > 1)
            {
                //characterModel.Move(currentObjectivesQueue.Peek().transform.position);
                characterModel.Move2(currentObjectivesQueue.Peek().transform.position, _smoothSpeed);
            }
            else
            {
              
                
                _eventFSM.SendInput(CharacterStates.Wait);
            }*/

            if(Vector2.Distance(nextPosition, transform.position) > 1.3f)
            {                
                characterModel.Move2(nextPosition, _smoothSpeed);
            }
            else
            {
                //_characterRigidbody.linearVelocity = Vector2.zero;
                _eventFSM.SendInput(CharacterStates.Idle);
            }
        };
        Moving.OnExit += x => { };

        #endregion

        #region WAIT STATE

        Wait.OnEnter += x =>
        {            
            characterView.OnIdle();
            Debug.Log("Entro aca");
            _characterRigidbody.linearVelocity = Vector2.zero;
            _currentState = CharacterStates.Wait;

            if (currentObjectivesQueue.Any())
            {
                if (currentObjectivesQueue.Peek().changeDirection)
                {
                    characterView.FlipCharacter();
                }

                if (currentObjectivesQueue.Peek().hasEvent)
                {
                    currentObjectivesQueue.Peek().pointEvent.Invoke();
                    currentObjectivesQueue.Dequeue();
                }
                else
                {
                    currentObjectivesQueue.Dequeue();
                    _eventFSM.SendInput(CharacterStates.Moving);
                }
            }
            
            
                        

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

            _currentState = CharacterStates.Jumping;
            characterView.OnJump();

        };

        Jumping.OnUpdate += () =>
        {
            //if (Physics2D.Raycast(transform.position, Vector2.down, 2,_floorLayerMask))
            //{
            //    _eventFSM.SendInput(CharacterStates.Moving);
            //}

        };
        Jumping.OnFixedUpdate += () =>
        {

        };
        Jumping.OnExit += x => { };

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

        _eventFSM.EnterFirstState();

       
    }

    protected virtual void Start()
    {
        instance = this;
        _pathFinding = new PathFindingThetaStar(_obstacleLayerMask);
        _characterRigidbody = GetComponent<Rigidbody2D>();
        yPositionOffset = Math.Abs(transform.position.y - feetPosition.position.y);
     
    }

    
    public virtual void Update()
    {
        _eventFSM.Update();

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

    public IEnumerator SendInputToFSM(CharacterStates newState, float time)
    {
        yield return new WaitForSeconds(time);
        _eventFSM.SendInput(newState);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(_currentState == CharacterStates.Jumping && collision.gameObject.layer == 6)
        {
            //_eventFSM.SendInput(CharacterStates.Moving);
            StartCoroutine(SendInputToFSM(CharacterStates.Moving, 0.2f));
            
        }
       
    }
}

public enum CharacterStates
{
    Idle,
    Moving,
    Wait,
    Stop,
    Jumping,
}
