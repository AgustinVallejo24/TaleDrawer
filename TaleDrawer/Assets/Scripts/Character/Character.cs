using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [SerializeField] bool _goToNextPosition;
    [SerializeField] public float yPositionOffset;
    [SerializeField] Transform feetPosition;
    [SerializeField] protected List<CustomNode> _currentPath;
    public Action onMovingStart;
    public Action onMovingEnd;
    public IInteractable currentInteractable;
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



    #region Level References
    public List<InterestPoint> pointList;
    public Queue<InterestPoint> currentObjectivesQueue = new Queue<InterestPoint>();
    public Queue<InterestPoint> mainObjectivesQueue;

    public Camera sceneCamera;
    [SerializeField] CustomNode _lookAtNode = null;
    [SerializeField] float _yDiffThreshold;

    public static Character instance;

    #endregion

    public bool test;

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
        StateConfigurer.Create(Idle)
            .SetTransition(CharacterStates.Moving, Moving)
             .SetTransition(CharacterStates.Jumping, Jumping)
            .Done();
        StateConfigurer.Create(Moving)
            .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Wait, Wait)
            //.SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Jumping, Jumping).Done();
        StateConfigurer.Create(Wait)
            .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Jumping, Jumping)
            .SetTransition(CharacterStates.JumpingToRope, JumpingToRope).Done();
        StateConfigurer.Create(Jumping)
            .SetTransition(CharacterStates.Idle, Idle)
             .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Landing, Landing)
            .Done();
        StateConfigurer.Create(Stop)
            .SetTransition(CharacterStates.Idle, Idle)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Wait, Wait).Done();
        StateConfigurer.Create(Landing)
            .SetTransition(CharacterStates.Moving, Moving)
            .SetTransition(CharacterStates.Wait, Wait)
            .SetTransition(CharacterStates.Stop, Stop)
            .SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(OnRope)
           .SetTransition(CharacterStates.Jumping, Jumping).Done();
        StateConfigurer.Create(JumpingToRope)
           .SetTransition(CharacterStates.OnRope, OnRope).Done();
        #endregion
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
            
            if (!_currentPath.Any())
            {

                _eventFSM.SendInput(CharacterStates.Idle);
            }
            else if (_currentPath.Count > 1)
            {
                //Quita el primer nodo si no tiene evento necesario, y si el personaje esta mas cerca del segundo nodo, que el primero del segundo.


                if (Vector2.Distance(new Vector2(_currentPath.First().transform.position.x, _currentPath.First().transform.position.y + yPositionOffset),
                    new Vector2(_currentPath.Skip(1).First().transform.position.x, _currentPath.Skip(1).First().transform.position.y + yPositionOffset)) >
                    Vector2.Distance(CustomTools.ToVector2(transform.position),
                    new Vector2(_currentPath.Skip(1).First().transform.position.x, _currentPath.Skip(1).First().transform.position.y + yPositionOffset)) 
                    &&  Math.Abs(_currentPath.First().transform.position.y - _currentPath.Skip(1).First().transform.position.y) < _yDiffThreshold)
                {
                    if (_currentPath.First().neighbours.Where(x => x.node == _currentPath.Skip(1).First()).First().nodeEvent.GetPersistentEventCount() == 0)
                    {                        
                        _currentPath.Remove(_currentPath.First());
                        _lookAtNode = _currentPath.First();
                    }
                    else
                    {
                        _lookAtNode = _currentPath.Skip(1).First();
                    }
                 
                }
                else
                {
                    _lookAtNode = _currentPath.First();
                }

                //if (!_currentPath.First().neighbours.Where(x => x.node == _currentPath.Skip(1).First()).First().canDoEvent &&
                //_currentPath.First().neighbours.Where(x => x.node == _currentPath.Skip(1).First()).First().nodeEvent.GetPersistentEventCount() == 0)
                //{
                //    if (Vector2.Distance(new Vector2(_currentPath.First().transform.position.x, _currentPath.First().transform.position.y + yPositionOffset),
                //        new Vector2(_currentPath.Skip(1).First().transform.position.x, _currentPath.Skip(1).First().transform.position.y + yPositionOffset)) >
                //        Vector2.Distance(CustomTools.ToVector2(transform.position),
                //        new Vector2(_currentPath.Skip(1).First().transform.position.x, _currentPath.Skip(1).First().transform.position.y + yPositionOffset)))
                //    {
                //        _currentPath.Remove(_currentPath.First());
                //        lookAtNode = _currentPath.First();
                //    }
                //    else if (!_currentPath.First().neighbours.Where(x => x.node == _currentPath.Skip(1).First()).First().canDoEvent &&
                //    _currentPath.First().neighbours.Where(x => x.node == _currentPath.Skip(1).First()).First().nodeEvent.GetPersistentEventCount() > 0)
                //    {
                //        lookAtNode = _currentPath.Skip(1).First();
                //    }
                //}
            }
            else
            {
                //Quita el nodo de la lista si solo hay uno, y ese nodo puede ver al punto en donde se hizo click.                
                if (!Physics2D.Raycast(_currentPath.First().transform.position, nextPosition - CustomTools.ToVector2(_currentPath.First().transform.position),
                            Vector2.Distance(CustomTools.ToVector2(_currentPath.First().transform.position), nextPosition), 10) && nextPosition != CustomTools.ToVector2(_currentPath.Last().transform.position))
                {                    
                    _goToNextPosition = true;
                    _currentPath.Remove(_currentPath.First());
                }
            }

            //Cambia la dirreccion del sprite.
            if (_currentPath.Any())
            {
                if(_lookAtNode != null)
                {
                    if (Mathf.Sign(_lookAtNode.transform.position.x - transform.position.x) > 0)
                    {
                        characterView.FlipCharacter(1);
                    }
                    else
                    {
                        characterView.FlipCharacter(-1);
                    }

                }
                else
                {
                    if (Mathf.Sign(_currentPath.First().transform.position.x - transform.position.x) > 0)
                    {
                        characterView.FlipCharacter(1);
                    }
                    else
                    {
                        characterView.FlipCharacter(-1);
                    }
                }
                
            }
            else
            {
                if (Mathf.Sign(nextPosition.x - transform.position.x) > 0)
                {
                    characterView.FlipCharacter(1);
                }
                else
                {
                    characterView.FlipCharacter(-1);
                }
            }



        };

        Moving.OnUpdate += () =>
        {


        };
        Moving.OnFixedUpdate += () =>
        {
            if (_currentPath.Any())
            {
                float sqrDistanceToTarget = (CustomTools.ToVector2(_currentPath.First().transform.position) - (new Vector2(transform.position.x, feetPosition.position.y))).sqrMagnitude;



                if (sqrDistanceToTarget > .5f)
                {

                    characterModel.Move2(_currentPath.First().transform.position, _smoothSpeed);

                }
                else
                {
                    if (_currentPath.Count > 1)
                    {

                        var neighbour = _currentPath.First().neighbours.Where(x => x.node == _currentPath.Skip(1).First()).First();
                        if (neighbour.nodeEvent.GetPersistentEventCount() > 0 && neighbour.canDoEvent)
                        {

                            _currentPath.Remove(_currentPath.First());
                            neighbour.nodeEvent.Invoke();
                        }
                        else
                        {
                            if (_currentPath.Count == 2)
                            {
                                if (Vector2.Distance(CustomTools.ToVector2(transform.position), new Vector2(nextPosition.x, transform.position.y))
                                < Vector2.Distance(new Vector2(_currentPath.Last().transform.position.x, transform.position.y), new Vector2(nextPosition.x, transform.position.y))
                                && nextPosition != CustomTools.ToVector2(_currentPath.Last().transform.position))
                                {
                                    _currentPath.Remove(_currentPath.Last());
                                    _goToNextPosition = true;
                                }
                            }
                            else
                            {
                                _currentPath.Remove(_currentPath.First());
                            }


                                

                            
                        }

                        if (Mathf.Sign(_currentPath.First().transform.position.x - transform.position.x) > 0)
                        {
                            characterView.FlipCharacter(1);
                        }
                        else
                        {
                            characterView.FlipCharacter(-1);
                        }

                    }
                    else if (_currentPath.Count == 1)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(_currentPath.First().transform.position, nextPosition - CustomTools.ToVector2(_currentPath.First().transform.position),
                            Vector2.Distance(CustomTools.ToVector2(_currentPath.First().transform.position), nextPosition), 10);
                        if (nextPosition != CustomTools.ToVector2(_currentPath.First().transform.position))
                        {
                            if (_currentPath.First().goalDelegate != null)
                            {
                                Debug.LogError(_currentPath.First().goalDelegate);
                                _currentPath.First().goalDelegate.Invoke();
                            }
                            else
                            {
                                Debug.LogError("NoTEngoFuncion");
                            }
                            if (!hit)
                            {
                                _goToNextPosition = true;

                                _currentPath.Remove(_currentPath.First());

                                if (Mathf.Sign(nextPosition.x - transform.position.x) > 0)
                                {
                                    characterView.FlipCharacter(1);
                                }
                                else
                                {
                                    characterView.FlipCharacter(-1);
                                }
                            }
                            else
                            {
                                Debug.Log(hit.transform.gameObject);
                                _currentPath.Remove(_currentPath.First());
                            }

                        }



                    }



                }
            }
            else
            {
                Debug.LogError("cuarto");
                if (_goToNextPosition)
                {
                    float sqrDistanceToTarget = (CustomTools.ToVector2(nextPosition) - (new Vector2(transform.position.x, feetPosition.position.y))).sqrMagnitude;



                    if (sqrDistanceToTarget > 1f)
                    {
                        //characterModel.Move2(nextPosition, _smoothSpeed);
                        characterModel.Move2(nextPosition, _smoothSpeed);

                    }
                    else
                    {
                        characterRigidbody.linearVelocity = Vector2.zero;
                        Debug.LogError("segundo");
                        _eventFSM.SendInput(CharacterStates.Idle);
                        onMovingEnd?.Invoke();
                    }
                }
                else
                {
                    characterRigidbody.linearVelocity = Vector2.zero;
                    Debug.LogError("tercero");
                    _eventFSM.SendInput(CharacterStates.Idle);
                    onMovingEnd?.Invoke();
                }

            }

        };
        Moving.OnExit += x =>
        {
            _goToNextPosition = false;
           

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

        OnRope.OnEnter += x =>
        {
            _currentState = CharacterStates.JumpingToRope;


        };

        OnRope.OnUpdate += () =>
        {


        };
        OnRope.OnFixedUpdate += () =>
        {

        };
        OnRope.OnExit += x => { };

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
        _pathFinding = new CustomPathfinding(_obstacleLayerMask);
        characterRigidbody = GetComponent<Rigidbody2D>();
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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {


        //if (_currentState == CharacterStates.Jumping && ((1 << collision.gameObject.layer) & _walkableLayerMask) != 0)
        //{
        //    _eventFSM.SendInput(CharacterStates.Landing);

        //    if (!_currentPath.First().shouldWait)
        //    {

        //        StartCoroutine(SendInputToFSM(CharacterStates.Moving, 0.2f));
        //    }
        //    else
        //    {
        //        StartCoroutine(SendInputToFSM(CharacterStates.Wait, 0.2f));
        //    }


        //}

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
        if(value == 0)
        {
            /*if (!first)
            {
                first = true;
                Debug.LogError("Comparo con el primero, y devuelvo " + ComparePathNodes(node, value, first));
            } */           
            return _currentPath.First().Equals(node);
        }
        else if(value == 1)
        {
            /*if (!first)
            {
                first = true;
                Debug.LogError("Comparo con el último, y devuelvo " + ComparePathNodes(node, value, first));
            }*/
            return _currentPath.Last().Equals(node);
        }
        else if(value == 2)
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
        CustomNode start = CustomTools.GetClosestNode(transform.position, SceneManager.instance.nodes);
        _currentPath = _pathFinding.AStar(start, goal);

        if (_currentPath.Count > 1 && _currentPath.SkipLast(1).Last().isClickable && Vector2.Distance(CustomTools.ToVector2(_currentPath.SkipLast(1).Last().transform.position), CustomTools.ToVector2(_currentPath.Last().transform.position))
            > Vector2.Distance(CustomTools.ToVector2(_currentPath.SkipLast(1).Last().transform.position), nextPosition) 
            && _currentPath.SkipLast(1).Last().neighbours.Where(x => x.node == _currentPath.Last()).Last().nodeEvent.GetPersistentEventCount() == 0)
        {
            _currentPath = _currentPath.SkipLast(1).ToList();
        }
        if (_currentPath.Any())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetPath(CustomNode goal)
    {
        CustomNode start = CustomTools.GetClosestNode(transform.position, SceneManager.instance.nodes);
        _currentPath = _pathFinding.AStar(start, goal);

        if (_currentPath.Any())
        {
            nextPosition = goal.transform.position;
            return true;
        }
        else
        {
            return false;
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
}

public enum CharacterStates
{
    Idle,
    Moving,
    Wait,
    Stop,
    Jumping,
    Landing,
    OnRope,
    JumpingToRope,
}
