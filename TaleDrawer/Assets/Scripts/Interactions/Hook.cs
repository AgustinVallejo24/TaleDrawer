using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hook : MonoBehaviour, IInteractable
{
    public RopeType myType;
    [SerializeField] SpawningObject _attachedObject;
    [SerializeField] public Soga rope;
    [SerializeField] SpawnableObjectType _posibleObjects;
    [SerializeField] Transform _hitch;
    [SerializeField] CustomNode _upperNode;
    [SerializeField] CustomNode _lowerNode;
    [SerializeField] CustomNode _leftNode;
    [SerializeField] CustomNode _rightNode;
    [SerializeField] Character _character;
    [SerializeField] bool _fromRight;
    [SerializeField] Transform _rightLandingPos;
    [SerializeField] Transform _leftLandingPos;
    [SerializeField] Transform _beforeRopeRightPos;
    [SerializeField] Transform _beforeRopeLeftPos;
    [SerializeField] InteractableType _interactableType;
    [SerializeField] public Transform tpPoint; 
    [SerializeField] float speedMultiplier;
    [SerializeField] Collider2D playerDetectionCollider;
    private void Start()
    {
        _attachedObject._currentInteractuable = this;

        if(_attachedObject.TryGetComponent<Soga>(out Soga soga))
        {
            rope = soga;            
        }       
    }
    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
        
    }

    public void Interact(SpawningObject spawningObject)
    {        
        if (_posibleObjects.HasFlag(spawningObject.myType))
        {
            if (myType == RopeType.Horizontal)
                playerDetectionCollider.enabled = true;

            Destroy(spawningObject.gameObject);

            _attachedObject.gameObject.SetActive(true);
            
            _lowerNode?.SetCanDoEvent(_upperNode, true);
            _leftNode?.SetCanDoEvent(_rightNode, true);
            _rightNode?.SetCanDoEvent(_leftNode, true);
        }
        else
        {
            spawningObject.CantInteract();
        }
        
    }

    public void Interact(GameObject interactor)
    {
        
    }

    public void InteractWithPlayer()
    {
        if(rope.myRopeType == RopeType.Vertical)
        {
            /*if (Mathf.Abs(_character.transform.position.y - _upperNode.transform.position.y) > Mathf.Abs(_character.transform.position.y - _lowerNode.transform.position.y))
            {
                _character.GetPath(_upperNode);
            }
            else
            {
                _character.GetPath(_lowerNode);
            }*/

            StartCoroutine(IGetOnRope());

        }
        else
        {
            /*if(Mathf.Abs(_character.transform.position.x - _rightNode.transform.position.x) > Mathf.Abs(_character.transform.position.x - _leftNode.transform.position.x))
            {
                
                _character.GetPath(_rightNode);
            }
            else
            {
                
                _character.GetPath(_leftNode);
            }*/

            _character.SendInputToFSM(CharacterStates.Moving);
        }

        
    }

    

    public void StartRopeEvent()
    {
        StartCoroutine(IGetOnRope());

    }

    public void IsFromRight(bool value)
    {
        _fromRight = value;
    }
    
    public void GetOnRope()
    {
        if (rope != null)
        {
            _character.currentHook = this;
            //_character.SendInputToFSM(CharacterStates.OnRope);
            
            
            if(myType == RopeType.Vertical)
            {
                VerticalMovement();
            }
            else
            {
                HorizontalMovement();
            }
        }
        else
        {
            _character.SendInputToFSM(CharacterStates.Idle);
        }
    }

    private void VerticalMovement()
    {
        //NewRopeEntering();
        _character.maxClimbingPos = rope.firstPoint.position;
        _character.minClimbingPos = rope.secondPoint.position;
        _character.climbingSpeedMultiplier = speedMultiplier;
        _character.characterView.OnEventMovement();
        _character.characterModel.Flip(rope.secondPoint.position);
        _character.transform.DOMoveX(rope.secondPoint.position.x, 0.2f).OnComplete(() => { _character.characterView.OnVerticalRopeEventMovement(); _character.characterView.FlipCharacter(1);
           StartCoroutine(_character.SendInputToFSM(CharacterStates.OnLadder, 0.2f));});
    }

    private void HorizontalMovement()
    {
        _character.characterRigidbody.gravityScale = 0;
        _character.SendInputToFSM(CharacterStates.JumpingToRope);
        _character.characterView.OnJumpingToRope();
        if (_fromRight)
        {
            rope.mySpRenderer.flipX = true;
            _character.characterModel.Jump(rope.firstPoint.position, () => {                
                _character.characterView.OnHorizontalRopeMovement(); _character.SendInputToFSM(CharacterStates.Swaying);
            }, false, 0.7f);
        }
        else
        {
            rope.mySpRenderer.flipX = false;
            _character.characterModel.Jump(rope.secondPoint.position, () => {                
                _character.characterView.OnHorizontalRopeMovement(); _character.SendInputToFSM(CharacterStates.Swaying);
            }, false, 0.7f);
        }
        
    }

    public void ExitingHorizontalRope(Transform characterRender)
    {
        
        if (_fromRight)
        {
            _character.transform.position = _beforeRopeLeftPos.position;
            characterRender.position = new Vector3(0, 0, 0);
            _character.SendInputToFSM(CharacterStates.JumpingToRope);            
            _character.characterModel.Jump(_leftLandingPos.position, () => { _character.characterRigidbody.gravityScale = 3; _character.SendInputToFSM(CharacterStates.Landing); _character.currentHook = null;
                StartCoroutine(_character.SendInputToFSM(CharacterStates.Moving, 0.25f));
            }, false, 0.7f, false);
        }
        else
        {
            _character.transform.position = _beforeRopeRightPos.position;
            characterRender.position = new Vector3(0, 0, 0);
            _character.SendInputToFSM(CharacterStates.JumpingToRope);            
            _character.characterModel.Jump(_rightLandingPos.position, () => { _character.characterRigidbody.gravityScale = 3; _character.SendInputToFSM(CharacterStates.Landing); _character.currentHook = null; 
                StartCoroutine(_character.SendInputToFSM(CharacterStates.Moving, 0.25f)); }, false, 0.7f, false);
        }
        

        
    }

    IEnumerator IGetOnRope()
    {
        _character.SendInputToFSM(CharacterStates.Wait);

        yield return null;

        GetOnRope();
        
    }
    public void RopeAnimationManager(int value)
    {
        if(value <= 0)
        {
            rope.myAnim.SetTrigger("Idle");
        }
        else
        {
            rope.myAnim.SetTrigger("Moving");
        }
    }

    public void RopeAnimatorSpeedController()
    {
        rope.myAnim.speed = _character.GetAnimatorSpeed();
    }

    public void NewRopeEntering()
    {
        _character.currentHook = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Character character) && myType == RopeType.Horizontal)
        {
            _character.characterRigidbody.gravityScale = 0;
            _character.characterRigidbody.linearVelocity = Vector2.zero;
            Debug.LogError("Detecto al pj");
            _character.currentHook = this;
            if (_character.transform.position.x > transform.position.x)
            {
                rope.mySpRenderer.flipX = true;
                Debug.LogError("Detecto al pj derecha");
                _character.characterView.OnHorizontalRopeMovement(); _character.SendInputToFSM(CharacterStates.Swaying);
                _fromRight = true;
            }
            else
            {
                Debug.LogError("Detecto al pj izquierda");
                _fromRight = false;
                rope.mySpRenderer.flipX = false;
                _character.characterView.OnHorizontalRopeMovement(); _character.SendInputToFSM(CharacterStates.Swaying);


            }

        }
    }
    public void InsideInteraction()
    {
      
    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
    }
}

[Serializable]
public enum RopeType
{
    Vertical,
    Horizontal
}
