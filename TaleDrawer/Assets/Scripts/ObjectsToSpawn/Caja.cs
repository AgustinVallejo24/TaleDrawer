using UnityEngine;
using System.Linq;
using DG.Tweening;
public class Caja : SpawningObject,IInteractable
{
    
    [SerializeField] LayerMask _clickableMask;
    [SerializeField] Transform _playerPos;

    [SerializeField] LayerMask _excludeLayers;
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
           _excludeLayers = _myColl.excludeLayers ;
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
        _myCharacter.onMovingEnd = null;

        
        _myCharacter.characterModel.Jump(_playerPos.position, () =>
        {
            // Llamar a OnLand
            _myColl.excludeLayers = default;
            _myCharacter.Land();
            // Ejecutar action si vino por parámetro
            _myCharacter.currentInteractable = this;
        });
        //_myCharacter.transform.DOJump(_playerPos.position, _myCharacter.currentJumpForce.y/3, 1, 1f).OnComplete(() =>
        //{
        //   //  Llamar a OnLand


        // //     Ejecutar action si vino por parámetro
        //    _myCharacter.currentInteractable = this;
        //    _myCharacter.onMovingEnd = null;
        //});
    }
    public void JumpOffBox()
    {
        Vector2 touchPos = SceneManager.instance._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        _myCharacter.currentInteractable = null;
        _myColl.excludeLayers = _excludeLayers;
        if (touchPos.x > transform.position.x)
        {
            _myCharacter.characterModel.Jump(transform.position + Vector3.right * 2,() =>
        {

                _myCharacter.Land();

            });
        }
        else
        {
            _myCharacter.characterModel.Jump(transform.position - Vector3.right * 2, () =>
            {

                _myCharacter.Land();

            });

        }
        

    }

    public void InsideInteraction()
    {
        
        JumpOffBox();
    }
}
