using UnityEngine;

public class Soga : SpawningObject, IInteractable
{
    [SerializeField] Hook hook;
    public Transform upperPoint;
    public Transform lowerPoint;
    [SerializeField] Animator _myAnim;

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
        if(_currentInteractuable != null)
        {
            _currentInteractuable.InteractWithPlayer();
        }
    }

    public void OnHasPlayer()
    {
        _myAnim.ResetTrigger("HasNotPlayer");
        _myAnim.SetTrigger("HasPlayer");
    }

    public void OnHasNotPlayer()
    {
        _myAnim.ResetTrigger("HasPlayer");
        _myAnim.SetTrigger("HasNotPlayer");
    }
}
