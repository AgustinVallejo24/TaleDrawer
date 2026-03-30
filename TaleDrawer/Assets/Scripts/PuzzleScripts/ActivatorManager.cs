using UnityEngine;
using UnityEngine.Events;

public class ActivatorManager : MonoBehaviour
{
    [SerializeField] protected MonkeyActivator[] activators;
    [SerializeField] public UnityEvent activationEvent;
    [SerializeField] protected bool hasCinematic;
    [SerializeField] protected bool needsInteraction;
    [SerializeField] ParticleSystem confetty;
    protected int currentActivatorsOn;
    private void Start()
    {
        foreach (var item in activators)
        {
            item.managerCheck += Activation; 
        }
    }
    public void Activation()
    {
        currentActivatorsOn++;
        if (!needsInteraction)
        {
            OnActivation();
        }
        
        if (currentActivatorsOn == activators.Length && !hasCinematic && !needsInteraction)
        {
            Debug.LogError("MeActivo");
            activationEvent?.Invoke();
        }
        else
        {
            Character.instance.SendInputToFSM(CharacterStates.Idle);
        }
       
    }

    public void ActivationViaInteraction()
    {
        if(currentActivatorsOn == activators.Length)
        {
            OnActivationViaInteraction();
            activationEvent?.Invoke();
        }
        else
        {
            Character.instance.SendInputToFSM(CharacterStates.Idle);
        }

    }

    public virtual void OnActivationViaInteraction()
    {

    }

    public void Confetti()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        confetty.Play();
    }
    public virtual void OnActivation()
    {

    }
}
