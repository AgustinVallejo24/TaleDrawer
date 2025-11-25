using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Collections;
public class Caja : SpawningObject,IInteractable
{
    
    [SerializeField] LayerMask _clickableMask;
    [SerializeField] Transform _playerPos;
    [SerializeField] LayerMask _eventMask;
    [SerializeField] LayerMask _excludeLayers;
    [SerializeField] Vector2 _jumpPosition;
    private void OnDestroy()
    {
        interactableDelegate?.Invoke(-weight,gameObject);
    }

    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
       
    }

    public void Interact(SpawningObject spawningObject)
    {
       
    }

    public void Interact(GameObject interactor)
    {
        
    }

    public void InteractWithPlayer()
    {
     
       if(Physics2D.OverlapCircle(transform.position,transform.localScale.x, _clickableMask))
        {
           _excludeLayers = _myColl.excludeLayers ;
             Vector2 newPos = _myColl.bounds.ClosestPoint(_myCharacter.transform.position);
            Debug.Log(newPos);
            newPos.y = transform.position.y;
            _myCharacter.GetPath(CustomTools.GetClosestNode(transform.position, SceneManager.instance.nodes.Where(x => x.isClickable == true).ToList()), newPos);
            _myCharacter.SendInputToFSM(CharacterStates.Moving);
            _myCharacter.onMovingEnd = JumpOverBox; 
        }
    }

    public void JumpOverBox()
    {
        _myCharacter.onMovingEnd = null;
        _myCharacter.currentInteractable = this;
        _myColl.excludeLayers = default;
        _myCharacter.SendInputToFSM(CharacterStates.Climb);
        _myCharacter.StartCoroutine(RunToCenter());

    }
    public void JumpOffBox()
    {
        Vector2 touchPos = SceneManager.instance._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        _myCharacter.currentInteractable = null;
        _myColl.excludeLayers = _excludeLayers;
        var eventObj = Physics2D.OverlapBox(transform.position,new Vector2(2,2),0,_eventMask).gameObject;
        BoxEvent boxE = default;
        if(eventObj!= null && eventObj.TryGetComponent(out BoxEvent boxEvent))
        {
            _jumpPosition = CustomTools.ToVector2(boxEvent.jumpPosition.position);
            boxE = boxEvent;
        }
        if(_jumpPosition != Vector2.zero)
        {
            if(Mathf.Abs(transform.position.x - _jumpPosition.x) > Mathf.Abs(_jumpPosition.x - touchPos.x))
            {
                if (boxE.shouldClimb)
                {
                    _myCharacter.characterModel.Jump(_jumpPosition, () =>
                    {

                        _myCharacter.SendInputToFSM(CharacterStates.Climb);

                    });
                }
                else
                {
                    _myCharacter.characterModel.Jump(_jumpPosition, () =>
                    {

                        _myCharacter.Land();

                    });
                }

            }
            else
            {
                if (touchPos.x > transform.position.x)
                {
                    _myCharacter.characterModel.Jump(transform.position + Vector3.right * 2, () =>
                    {

                        _myCharacter.Land();

                    });
                }
                else
                {
                    _myCharacter.characterModel.Jump(transform.position - Vector3.right * 2, () =>
                    {

                        _myCharacter.Land();

                    });

                }
            }
        }
        else
        {
            if (touchPos.x > transform.position.x)
            {
                _myCharacter.characterModel.Jump(transform.position + Vector3.right * 2, () =>
                {

                    _myCharacter.Land();

                });
            }
            else
            {
                _myCharacter.characterModel.Jump(transform.position - Vector3.right * 2, () =>
                {

                    _myCharacter.Land();

                });

            }
        }

        

    }
    public IEnumerator RunToCenter()
    {
        yield return new WaitForSeconds(.9f);
        _myCharacter.characterView.OnMove();
        _myCharacter.transform.DOMove(new Vector2(transform.position.x, _myCharacter.transform.position.y), .3f);
        yield return new WaitForSeconds(.3f);
        _myCharacter.characterView.OnIdle();
       _myCharacter.SendInputToFSM(CharacterStates.Wait);

    }
    
    public void InsideInteraction()
    {
        
        JumpOffBox();
    }
}
