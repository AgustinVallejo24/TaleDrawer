using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
public class MonkeyActivator : Activator, IInteractable
{
    public Action managerCheck;
    public bool isActive;
    [SerializeField] LayerMask _clickableMask;
    [SerializeField] Transform _playerPos;
    [SerializeField] InteractableType _interactableType;
    [SerializeField] GameObject _onStatue;
    [SerializeField] GameObject _offStatue;

    public override void Activation()
    {
        base.Activation();

        isActive = true;
        _onStatue.SetActive(true);
        _offStatue.SetActive(false);
        managerCheck.Invoke();
        Character myCharacter = Character.instance;
        myCharacter.currentActivator = null;
       
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
        Character myCharacter = Character.instance;
        myCharacter.characterView.OnEventMovement();
        myCharacter.characterModel.Flip(new Vector3(_playerPos.position.x, myCharacter.transform.position.y, 0));
        float distance = Mathf.Abs(_playerPos.position.x - myCharacter.transform.position.x);
        myCharacter.transform.DOMoveX(_playerPos.position.x, distance/4).OnComplete(() =>
        {
            myCharacter.currentActivator = this;
            myCharacter.characterView.OnIdle();
            myCharacter.characterModel.Flip(new Vector3(transform.position.x, myCharacter.transform.position.y, 0));
            myCharacter.SendInputToFSM(CharacterStates.DoingEvent);
            myCharacter.SetAnimatorTrigger("HighFive");

        });
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
