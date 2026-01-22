using UnityEngine;
using System.Linq;
using System.Collections;
using DG.Tweening;
public class Lever : Activator, IInteractable
{

    [SerializeField] Character _myCharacter;
    [SerializeField] Animator _animator;
    [SerializeField] bool _leverState;
    [SerializeField] Animator _door;
    [SerializeField] Transform _playerPos;
    [SerializeField] InteractableType _interactableType;
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
        ActivatePlayerAnimation();
    }

    public void ActivatePlayerAnimation()
    {
        Character.instance.characterView.OnMove();
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
            _door.SetTrigger("Open");
           // var beforeN = new NeighbouringNodesAndActions();
            //beforeN.node = _afterDoor;
            //beforeN.nodeEvent = new UnityEngine.Events.UnityEvent();
       //     var afterN = new NeighbouringNodesAndActions();
            //afterN.node = _beforeDoor;
            //afterN.nodeEvent = new UnityEngine.Events.UnityEvent();
            //_beforeDoor.neighbours.Add(beforeN);
            //_afterDoor.neighbours.Add(afterN);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
