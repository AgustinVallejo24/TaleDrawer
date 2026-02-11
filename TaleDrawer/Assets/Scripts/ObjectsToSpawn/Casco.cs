using UnityEngine;

public class Casco : SpawningObject, IDeletable, IInteractable
{
    [SerializeField] public int _health;
    [SerializeField] public Quaternion neededRot;
    [SerializeField] bool _isBeingUsed;
    [SerializeField] InteractableType _interactableType;
    [SerializeField] Collider2D _extraColl;


    public override void InteractionWithEntity()
    {
        if(!Character.instance._hasHelmet && !_isBeingUsed)
        {
            Character.instance.PutOnHelmet();
            Delete();
        }


    }
    public override void Paint()
    {
        StartCoroutine(GetComponentInChildren<Paint>().PaintSprite());
    }
    public void InstantPaint()
    {
        GetComponentInChildren<Paint>().InstantPaint();
    }
    public override void Delete()
    {
        base.Delete();

        if (_isBeingUsed)
        {
            Character.instance._hasHelmet = false;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
            
    }

    public override void OnSpawned()
    {
        base.OnSpawned();
        gameObject.layer = 11;
        _extraColl.enabled = true;
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
        InteractionWithEntity();
    }

    public void InsideInteraction()
    {
        
    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
    }
}
