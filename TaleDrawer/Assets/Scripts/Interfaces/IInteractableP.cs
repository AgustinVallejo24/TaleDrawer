using UnityEngine;

public interface IInteractableP
{
    public void Interact();

    public void OnLeavingInteraction();

    public KeyCode InteractionKey();

    public InteractableType MyInteractableType();
}
