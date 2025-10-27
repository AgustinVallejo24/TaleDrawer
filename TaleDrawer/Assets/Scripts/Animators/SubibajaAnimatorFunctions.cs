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
    public void ChangeLeftNeightbours(int value)
    {
        Debug.LogError("Entro al evento de animacion");
        subibaja.ChangeLeftNeightbours(value);
    }
    public void ChangeRightNeightbours(int value)
    {
        subibaja.ChangeRightNeightbours(value);
    }

}
