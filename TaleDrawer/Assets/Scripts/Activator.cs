using UnityEngine;
using System;
using System.Collections;
using System.Linq;
public class Activator : MonoBehaviour, IInteractable
{
    public Action managerCheck;
    public bool isActive;
    [SerializeField] LayerMask _clickableMask;
    [SerializeField] Transform _playerPos;
    [SerializeField] InteractableType _interactableType;
    public void Activation()
    {
        isActive = true;
        GetComponent<SpriteRenderer>().color = Color.red;
        managerCheck.Invoke();
    }

    public void InsideInteraction()
    {
       
    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
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
        if (!isActive)
        {
            ActivatePlayerAnimation();
        }

    }

    public void ActivatePlayerAnimation()
    {
        Character _myCharacter = Character.instance;
        _myCharacter.SendInputToFSM(CharacterStates.DoingEvent);
        StartCoroutine(ActivationCoroutine());
       // _myCharacter.SetAnimatorTrigger("PullLever");
    }

    public IEnumerator ActivationCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        Activation();
        Character _myCharacter = Character.instance;
        _myCharacter.SendInputToFSM(CharacterStates.Idle);
    }
}
