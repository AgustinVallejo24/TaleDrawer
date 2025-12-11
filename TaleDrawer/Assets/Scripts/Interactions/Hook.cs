using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hook : MonoBehaviour, IInteractable
{
    [SerializeField] SpawningObject _attachedObject;
    [SerializeField] public Soga rope;
    [SerializeField] SpawnableObjectType _posibleObjects;
    [SerializeField] Transform _hitch;
    [SerializeField] CustomNode _upperNode;
    [SerializeField] CustomNode _lowerNode;
    [SerializeField] Character _character;

    private void Start()
    {
        _attachedObject._currentInteractuable = this;

        if(_attachedObject.TryGetComponent<Soga>(out Soga soga))
        {
            rope = soga;
        }

        _lowerNode.SetCanDoEvent(_upperNode, true);
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

    

    public void StartRopeEvent()
    {
        StartCoroutine(IGetOnRope());

    }
    
    public void GetOnRope()
    {
        if (rope != null)
        {
            _character.currentHook = this;
            _character.SendInputToFSM(CharacterStates.OnRope);
            _character.characterView.OnMove();
            _character.transform.DOMove(rope.lowerPoint.position, 0.2f).OnComplete(() => _character.characterView.OnRopeEventMovement());
        }
        else
        {
            _character._currentPath = null;
            _character.SendInputToFSM(CharacterStates.Idle);
        }
    }

    IEnumerator IGetOnRope()
    {
        _character.SendInputToFSM(CharacterStates.Wait);

        yield return new WaitForSeconds(0.2f);

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
