using UnityEngine;

public class SubibajaAnimatorFunctions : MonoBehaviour
{
    [SerializeField] Subibaja subibaja;





    public void OnMovingStateChange(int sign)
    {
        if(sign == 0)
        {
            subibaja.OnMovingStateChange(false);
        }
        else
        {
            subibaja.OnMovingStateChange(true);
        }
       
    }
}
