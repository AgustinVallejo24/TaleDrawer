using UnityEngine;
using UnityEngine.Events;

public class ActivatorManager : MonoBehaviour
{
    [SerializeField] protected MonkeyActivator[] activators;
    [SerializeField] UnityEvent activationEvent;
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
        OnActivation();
        if (currentActivatorsOn == activators.Length)
        {
            Debug.LogError("MeActivo");
            activationEvent?.Invoke();
        }
        else
        {
            Character.instance.SendInputToFSM(CharacterStates.Idle);
        }
       
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
