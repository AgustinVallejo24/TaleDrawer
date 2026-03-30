using System;
using UnityEngine;

public class Gem : Activator, IInteractableP
{
    [SerializeField] InteractableType _interactableType;
    [SerializeField] ActivatorManager _myManager;
    [SerializeField] public Collider2D myCollider;
    public Action _onActivation;
    public override void Activation()
    {
        base.Activation();
        _onActivation?.Invoke();
        _myManager.Activation();
        gameObject.SetActive(false);
    }

    public override void AutoCompletePuzzle()
    {
        base.AutoCompletePuzzle();
        Activation();
    }

    public void Interact()
    {
        Activation();
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
