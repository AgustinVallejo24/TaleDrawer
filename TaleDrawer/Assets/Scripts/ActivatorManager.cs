using UnityEngine;
using UnityEngine.Events;

public class ActivatorManager : MonoBehaviour
{
    [SerializeField] Activator[] activators;
    [SerializeField] UnityEvent activationEvent;
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

    public virtual void OnActivation()
    {

    }
}
