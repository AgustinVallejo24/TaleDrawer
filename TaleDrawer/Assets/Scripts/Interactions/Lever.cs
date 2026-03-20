using UnityEngine;
using System.Linq;
using System.Collections;
using DG.Tweening;
using UnityEngine.Events;
public class Lever : Activator, IInteractableP
{

    [SerializeField] Character _myCharacter;
    [SerializeField] Animator _animator;
    [SerializeField] bool _leverState;
    [SerializeField] Animator _animToActivate;
    [SerializeField] Transform _playerPos;
    [SerializeField] InteractableType _interactableType;
    [SerializeField] GameObject _eKey;
    [SerializeField] UnityEvent _leverEvent;
    
    public override void AutoCompletePuzzle()
    {
        base.AutoCompletePuzzle();
        _animator.SetTrigger("Down");
        _leverState = true;
        _leverEvent.Invoke();
        completed = true;
        GetComponent<Collider2D>().enabled = false;
    }

    public void Interact()
    {
        ActivatePlayerAnimation();
    }

    public void ActivatePlayerAnimation()
    {
        Character.instance.characterView.OnMove();
        Character.instance.HideKeyUI();
        Character.instance.transform.DOMoveX(transform.position.x, 0.2f).OnComplete(() => 
        {
            _myCharacter.currentActivator = this;
            _myCharacter.SendInputToFSM(CharacterStates.DoingEvent);
            _myCharacter.SetAnimatorTrigger("PullLever");

        });

    }
    public override void Activation()
    {
        base.Activation();
        StartCoroutine(ActivateLeverCoroutine());
    }
    public void ActivateLever()
    {
        StartCoroutine(ActivateLeverCoroutine());
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public IEnumerator ActivateLeverCoroutine()
    {
        if(_myCharacter.currentActivator == this)
        _myCharacter.currentActivator = null;
        
        if (_leverState)
        {
            //_animator.SetTrigger("Up");
        }
        else
        {

            _animator.SetTrigger("Down");
            _leverState = true;
            yield return new WaitForSeconds(1f);
            _leverEvent.Invoke();
            completed = true;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public InteractableType MyInteractableType()
    {
        return InteractableType.Event;
    }

    public void OnLeavingInteraction()
    {
        throw new System.NotImplementedException();
    }

    public KeyCode InteractionKey()
    {
        return KeyCode.E;
    }
}
