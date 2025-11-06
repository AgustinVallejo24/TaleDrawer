using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hook : MonoBehaviour, IInteractable
{
    [SerializeField] SpawningObject _attachedObject;
    [SerializeField] SpawnableObjectType _posibleObjects;
    [SerializeField] Transform _hitch;
    [SerializeField] CustomNode _upperNode;
    [SerializeField] CustomNode _lowerNode;
    [SerializeField] Character _character;

    private void Start()
    {
        
    }
    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
        
    }

    public void Interact(SpawningObject spawningObject)
    {        
        if (_posibleObjects.HasFlag(spawningObject.myType))
        {
            _attachedObject = spawningObject;
            if(_attachedObject._myrb != null)
            {
                _attachedObject._myrb.gravityScale = 0;
            }
            _attachedObject._myColl.isTrigger = true;
            _attachedObject.gameObject.layer = 11;
            _attachedObject.transform.position = new Vector2(_hitch.transform.position.x, _hitch.transform.position.y - _attachedObject.transform.localScale.y);
            _lowerNode.SetCanDoEvent(_upperNode, true);
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
        Debug.LogError(_attachedObject.name);
        if (Mathf.Abs(_character.transform.position.y - _upperNode.transform.position.y) > Mathf.Abs(_character.transform.position.y - _lowerNode.transform.position.y))
        {
            _character.GetPath(_upperNode);
        }
        else
        {
            _character.GetPath(_lowerNode);
        }

        _character.SendInputToFSM(CharacterStates.Moving);
    }

    public void RopeMovement(Transform pointI, Transform pointF, Soga soga)
    {
        _character.SendInputToFSM(CharacterStates.OnRope);
        bool nextAction = _character.ComparePathNodes(_upperNode, 1);
        soga.OnHasPlayer();
        _character.transform.DOMoveY(pointF.position.y, pointF.position.y - pointI.position.y)
            .OnComplete(() => { _character.characterModel.Jump(_upperNode._jumpPosition.transform.position, 
                () => { if (nextAction && CustomTools.ToVector2(_upperNode.transform.position) == _character.nextPosition) { _character.ClearPath();/*_character.SendInputToFSM(CharacterStates.Wait);*/ } 
                    _character.Land();
                    _character.characterRigidbody.gravityScale = 1; });
                _character.characterView.OnExitingRope(); soga.OnHasNotPlayer(); 
            });
    }

    public void StartRopeEvent()
    {
        StartCoroutine(IGetOnRope());
    }
    public void GetOnRope()
    {
        if(_attachedObject.TryGetComponent(out Soga soga))
        {
            _character.SendInputToFSM(CharacterStates.JumpingToRope);
            _character.characterModel.Jump(soga.lowerPoint.position, 
                () => { _character.characterRigidbody.gravityScale = 0; _character.characterView.OnWaitingForRopeMovement();
                    StartCoroutine(StartRopeMovement(soga.lowerPoint, soga.upperPoint, soga)); }, false);
        }
        
    }

    IEnumerator StartRopeMovement(Transform p1, Transform p2, Soga soga)
    {
        _character.transform.DOKill();
        yield return new WaitForSeconds(0.2f);
        RopeMovement(p1, p2, soga);
    }

    public IEnumerator IGetOnRope()
    {
        _character.SendInputToFSM(CharacterStates.Wait);
        yield return new WaitForSeconds(0.2f);

        GetOnRope();
    }
    
}
