using UnityEngine;

public class Soga : SpawningObject, IInteractable
{
    [SerializeField] Hook hook;
    public Transform firstPoint;
    public Transform secondPoint;
    public RopeType myRopeType;

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

    public void InsideInteraction()
    {
       
    }

    public void OnErased()
    {
        gameObject.SetActive(false);
    }
}
