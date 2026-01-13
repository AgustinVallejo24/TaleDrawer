using UnityEngine;
using System.Collections.Generic;
public class Net : MonoBehaviour, IInteractable
{
    [SerializeField] Polea _polea;
    [SerializeField] List<ObjectsAndTypes> objectsAndTypes;
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
        SpawningObject sP = interactor.GetComponent<SpawningObject>();
        if(sP.weight>0)
        {
            _polea.netWeight = sP.weight;
            _polea.CheckWeight();
        }
        Destroy(interactor);
        foreach (var item in objectsAndTypes)
        {
            if(objectType == item.type)
            {
                item.poleyObject.Activation(true);
            }
            else
            {
                item.poleyObject.Activation(false);
            }
        }
    }


    public void Interact(SpawningObject spawningObject)
    {
        
    }

    public void Interact(GameObject interactor)
    {
        
    }

    public void InteractWithPlayer()
    {
        throw new System.NotImplementedException();
    }

    
}
[System.Serializable]
public struct ObjectsAndTypes
{
    public PoleyObject poleyObject;
    public SpawnableObjectType type;
}