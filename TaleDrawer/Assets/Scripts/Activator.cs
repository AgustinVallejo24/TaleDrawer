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
    public void Activation()
    {
        isActive = true;
        managerCheck.Invoke();
    }

    public void InsideInteraction()
    {
       
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
        Character _myCharacter = Character.instance;
        if (Physics2D.OverlapCircle(transform.position, transform.localScale.x, _clickableMask) && !isActive)
        {
            Vector3 pos = (new Vector3(_myCharacter.transform.position.x, _playerPos.position.y, 0) - _playerPos.transform.position).normalized;
            _myCharacter.SendInputToFSM(CharacterStates.Moving);
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
