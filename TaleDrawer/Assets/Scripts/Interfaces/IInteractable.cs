using UnityEngine;

public interface IInteractable 
{
    public void Interact(SpawnableObjectType objectType, GameObject interactor);
    public void Interact(SpawningObject spawningObject);
    public void Interact(GameObject interactor);

    public void InteractWithPlayer();

}
