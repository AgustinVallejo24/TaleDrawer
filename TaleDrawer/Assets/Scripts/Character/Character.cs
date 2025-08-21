using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Character : MonoBehaviour
{
    #region Variables

    [SerializeField] float _maxSpeed;
    public float currentSpeed;
    [SerializeField] float _maxLife;
    protected float _currentLife;
    [SerializeField] float _jumpForce;
    public float currentJumpForce;
    [SerializeField] protected LayerMask _obstacleLayerMask;

    #endregion

    #region References

    [SerializeField] protected Rigidbody2D _characterRigidbody;
    protected Animator _animator;
    public CharacterStates _currentState;

    #endregion


    #region Side Scripts References

    PathFindingThetaStar _pathFinding;
    protected EventFSM<CharacterStates> _eventFSM;
    protected CharacterModel _characterModel;
    protected CharacterView _characterView;
    #endregion



    #region Level References
    public List<InterestPoint> pointList;
    public Queue<InterestPoint> currentObjectivesQueue = new Queue<InterestPoint>();
    public Queue<InterestPoint> mainObjectivesQueue;

    #endregion

    protected virtual void Awake()
    {
     
        foreach (var item in pointList)
        {
            currentObjectivesQueue.Enqueue(item);
        }
        var Idle = new StateE<CharacterStates>("Idle");
        var Moving = new StateE<CharacterStates>("Moving");
        StateConfigurer.Create(Idle).SetTransition(CharacterStates.Moving, Moving).SetTransition(CharacterStates.Idle, Idle).Done();
        StateConfigurer.Create(Moving).SetTransition(CharacterStates.Idle, Idle).Done();

        // _eventFSM.SendInput(CharacterStates.Idle);
        _eventFSM = new EventFSM<CharacterStates>(Idle);
        #region IDLE STATE

        Idle.OnEnter += x =>
        {
            
            _currentState = CharacterStates.Idle;
            if (currentObjectivesQueue.Any())
            {
                _eventFSM.SendInput(CharacterStates.Moving);
            }
            else
            {
               
            }
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


        };

        Moving.OnUpdate += () => 
        {
            
            if (Vector2.Distance(currentObjectivesQueue.Peek().transform.position, transform.position) > 1)
            {

                _characterModel.Move(currentObjectivesQueue.Peek().transform.position);
            }
            else
            {
                currentObjectivesQueue.Dequeue();
            }
        };

        Moving.OnExit += x => { };

        #endregion
        _eventFSM.EnterFirstState();

       
    }

    protected virtual void Start()
    {

        _pathFinding = new PathFindingThetaStar(_obstacleLayerMask);
        _characterRigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
       
     
    }

    
    public virtual void Update()
    {
         _eventFSM.Update();
    }
}

public enum CharacterStates
{
    Idle,
    Moving,
}
