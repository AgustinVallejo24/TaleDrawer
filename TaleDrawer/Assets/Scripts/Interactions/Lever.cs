using UnityEngine;
using System.Linq;
using System.Collections;
public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] LayerMask _clickableMask;
    [SerializeField] Character _myCharacter;
    [SerializeField] Animator _animator;
    [SerializeField] bool _leverState;
    [SerializeField] Animator _door;
    [SerializeField] Transform _playerPos;
    [SerializeField] CustomNode _beforeDoor;
    [SerializeField] CustomNode _afterDoor;
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
        if (Physics2D.OverlapCircle(transform.position, transform.localScale.x, _clickableMask) && !_leverState)
        {
            Vector3 pos = (new Vector3(_myCharacter.transform.position.x, _playerPos.position.y, 0) - _playerPos.transform.position).normalized;


        }
    }

    public void ActivatePlayerAnimation()
    {
        _myCharacter.currentLever = this;
        _myCharacter.SendInputToFSM(CharacterStates.DoingEvent);
        _myCharacter.SetAnimatorTrigger("PullLever");
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
        _myCharacter.currentLever = null;
        
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
            var beforeN = new NeighbouringNodesAndActions();
            beforeN.node = _afterDoor;
            beforeN.nodeEvent = new UnityEngine.Events.UnityEvent();
            var afterN = new NeighbouringNodesAndActions();
            afterN.node = _beforeDoor;
            afterN.nodeEvent = new UnityEngine.Events.UnityEvent();
            _beforeDoor.neighbours.Add(beforeN);
            _afterDoor.neighbours.Add(afterN);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
