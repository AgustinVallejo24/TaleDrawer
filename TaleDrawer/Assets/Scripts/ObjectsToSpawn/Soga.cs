using UnityEngine;
using UnityEngine.UI;

public class Soga : SpawningObject, IInteractable
{
    [SerializeField] Hook hook;
    public Transform firstPoint;
    public Transform secondPoint;
    public RopeType myRopeType;
    public SpriteRenderer mySpRenderer;
    [SerializeField] InteractableType _interactableType;

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
    public override void Paint()
    {
        StartCoroutine(GetComponentInChildren<Paint>().PaintSprite());
    }
    private void Update()
    {
        if(myRopeType == RopeType.Vertical && hook != null)
        {
            hook.RopeAnimatorSpeedController();
        }
    }

    public override void Delete()
    {
        base.Delete();

        hook.Delete();
    }
}
