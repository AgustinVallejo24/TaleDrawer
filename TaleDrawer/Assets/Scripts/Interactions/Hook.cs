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
            _attachedObject.gameObject.layer = 11;
            _attachedObject.transform.position = new Vector2(_hitch.transform.position.x, _hitch.transform.position.y - _attachedObject.transform.localScale.y);
            _lowerNode.SetCanDoEvent(_upperNode, true);
        }
        else
        {
            spawningObject._currentInteractuable = null;
            spawningObject._intrectableName = "";
        }
        
    }

    public void Interact(GameObject interactor)
    {
        
    }

    public void InteractWithPlayer()
    {
        Debug.LogError(_attachedObject.name);
        if (Vector3.Distance(_character.transform.position, _upperNode.transform.position) > (Vector3.Distance(_character.transform.position, _lowerNode.transform.position)))
        {
            _character.GetPath(_upperNode);
        }
        else
        {
            _character.GetPath(_lowerNode);
        }

        _character.SendInputToFSM(CharacterStates.Moving);
    }

    public void RopeMovement(Transform pointI, Transform pointF)
    {
        _character.SendInputToFSM(CharacterStates.OnRope);

        _character.transform.DOMoveY(pointF.position.y, pointF.position.y - pointI.position.y)
            .OnComplete(() => { _character.characterModel.Jump(_upperNode.transform.position, () => { _character.SendInputToFSM(CharacterStates.Wait); _character.characterRigidbody.gravityScale = 1; }, false);
                _character.characterView.OnExitingRope();});
    }

    public void GetOnRope()
    {
        if(_attachedObject.TryGetComponent(out Soga soga))
        {
            _character.SendInputToFSM(CharacterStates.JumpingToRope);
            _character.characterModel.Jump(soga.lowerPoint.position, 
                () => { _character.characterRigidbody.gravityScale = 0; _character.characterView.OnWaitingForRopeMovement();
                    StartCoroutine(StartRopeMovement(soga.lowerPoint, soga.upperPoint)); }, false);
        }
        
    }

    IEnumerator StartRopeMovement(Transform p1, Transform p2)
    {
        yield return new WaitForSeconds(0.5f);

        RopeMovement(p1, p2);
    }
    
}
