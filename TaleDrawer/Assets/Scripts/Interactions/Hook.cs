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


        }

        _character.SendInputToFSM(CharacterStates.Moving);
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
            _character.SendInputToFSM(CharacterStates.OnRope);
            
            
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
        _character.characterView.OnMove();
        _character.transform.DOMove(rope.secondPoint.position, 0.2f).OnComplete(() => _character.characterView.OnVerticalRopeEventMovement());
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
            _character.characterModel.Jump(_leftLandingPos.position, () => { _character.characterRigidbody.gravityScale = 1; _character.SendInputToFSM(CharacterStates.Landing); _character.currentHook = null;
                StartCoroutine(_character.SendInputToFSM(CharacterStates.Moving, 0.25f));
            }, false, 0.7f, false);
        }
        else
        {
            _character.transform.position = _beforeRopeRightPos.position;
            characterRender.position = new Vector3(0, 0, 0);
            _character.SendInputToFSM(CharacterStates.JumpingToRope);            
            _character.characterModel.Jump(_rightLandingPos.position, () => { _character.characterRigidbody.gravityScale = 1; _character.SendInputToFSM(CharacterStates.Landing); _character.currentHook = null; 
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

    

    public void InsideInteraction()
    {
      
    }
}

[Serializable]
public enum RopeType
{
    Vertical,
    Horizontal
}
