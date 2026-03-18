using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator _myAnim;
    
    public void DoorActivation()
    {
        _myAnim.SetTrigger("Open");
    }
}
