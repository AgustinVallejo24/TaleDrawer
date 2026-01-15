using UnityEngine;
using UnityEngine.Events;

public class ActivatorManager : MonoBehaviour
{
    [SerializeField] Activator[] activators;
    [SerializeField] UnityEvent activationEvent;
    [SerializeField] ParticleSystem confetty;
    int currentActivatorsOn;
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
