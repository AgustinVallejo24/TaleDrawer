using UnityEngine;
using System.Linq;
using DG.Tweening;
public class Caja : SpawningObject,IInteractable
{
    [SerializeField] LayerMask _objectMask;
    [SerializeField] LayerMask _clickableMask;
    [SerializeField] Transform _playerPos;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(SpawnableObjectType.Caja, gameObject);
        }
        if (collision.gameObject.TryGetComponent(out SpawningObject spawningObject) && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y + .5f, _objectMask) && spawningObject.weight > 1f)
        {
            Destroy(spawningObject.gameObject);
        }

    }

    private void OnDestroy()
    {
        interactableDelegate?.Invoke(-weight,gameObject);
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
     
       if(Physics2D.OverlapCircle(transform.position,transform.localScale.x, _clickableMask))
        {
           
            Vector2 newPos = _myColl.bounds.ClosestPoint(_myCharacter.transform.position);
            Debug.Log(newPos);
            newPos.y = _myColl.bounds.min.y;
            if(_myCharacter.transform.position.x > transform.position.x)
            {
                newPos.x += 0.3f;
                
            }
            else
            {
                newPos.x -= 0.3f;
            }
            _myCharacter.GetPath(CustomTools.GetClosestNode(transform.position, SceneManager.instance.nodes.Where(x => x.isClickable == true).ToList()), newPos);
            _myCharacter.SendInputToFSM(CharacterStates.Moving);
            _myCharacter.onMovingEnd = JumpOverBox; 
        }
    }

    public void JumpOverBox()
    {
        _myColl.excludeLayers = default;
        _myCharacter.transform.DOJump(_playerPos.position, _myCharacter.currentJumpForce.y/3, 1, 1f).OnComplete(() =>
        {
            // Llamar a OnLand


            // Ejecutar action si vino por parámetro
            _myCharacter.onMovingEnd = null;
        });
    }
}
