using UnityEngine;
using System.Linq;
public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] LayerMask _clickableMask;
    [SerializeField] Character _myCharacter;
    [SerializeField] Animator _animator;
    [SerializeField] bool _leverState;
    [SerializeField] Animator _door;
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
        if (Physics2D.OverlapCircle(transform.position, transform.localScale.x, _clickableMask))
        {
            _myCharacter.GetPath(CustomTools.GetClosestNode(transform.position, SceneManager.instance.nodes.Where(x => x.isClickable == true).ToList()), transform.position);
            _myCharacter.SendInputToFSM(CharacterStates.Moving);
            _myCharacter.onMovingEnd = ActivatePlayerAnimation;
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
        if (_leverState)
        {
            _animator.SetTrigger("Up");
        }
        else
        {
            _animator.SetTrigger("Down");
            _door.SetTrigger("Open");
        }
        _myCharacter.currentLever = null;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
