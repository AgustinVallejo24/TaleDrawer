using UnityEngine;

public class Hook : MonoBehaviour, IInteractable
{
    [SerializeField] SpawningObject _attachedObject;
    [SerializeField] SpawnableObjectType _posibleObjects;
    [SerializeField] Transform _hitch;
    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
        
    }

    public void Interact(SpawningObject spawningObject)
    {
        Debug.LogError(spawningObject.name);
        if (_posibleObjects.HasFlag(spawningObject.myType))
        {
            _attachedObject = spawningObject;
            _attachedObject.transform.position = new Vector2(_hitch.transform.position.x, _hitch.transform.position.y - _attachedObject.transform.localScale.y);
        }
        else
        {
            spawningObject._currentInteractuable = null;
            spawningObject._intrectableName = "";
        }
        
    }

    public void Interact(GameObject interactor)
    {
        
    }

    public void InteractWithPlayer()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
