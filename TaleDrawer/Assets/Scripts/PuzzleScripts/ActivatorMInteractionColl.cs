using UnityEngine;

public class ActivatorMInteractionColl : MonoBehaviour, IInteractableP
{
    [SerializeField] ActivatorManager _myManager;
    [SerializeField] InteractableType _interactableType;

    public void Interact()
    {
        if( _myManager != null)
        {
            _myManager.ActivationViaInteraction();
        }
    }

    public KeyCode InteractionKey()
    {
        return KeyCode.E;
    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
    }

    public void OnLeavingInteraction()
    {
        
    }
    
}
