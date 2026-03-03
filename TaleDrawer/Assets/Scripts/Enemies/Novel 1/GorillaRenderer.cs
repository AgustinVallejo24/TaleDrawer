using UnityEngine;

public class GorillaRenderer : MonoBehaviour
{
    [SerializeField] Gorilla _myGorilla;
    [SerializeField] Animator _myAnim;

    public void Attack()
    {
        _myGorilla.Attack();
    }

    public void StartFloorDetetction()
    {
        _myGorilla.floorDetection = true;
    }

    public void ShortStunActivation(int value)
    {
        if(value < 0)
        {
            _myGorilla.ShortStunActivationState(true);
        }
        else
        {
            _myGorilla.ShortStunActivationState(false);
        }
        
    }
    
}
