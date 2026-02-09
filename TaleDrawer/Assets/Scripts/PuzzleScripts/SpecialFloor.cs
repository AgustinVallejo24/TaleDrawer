using UnityEngine;

public class SpecialFloor : MonoBehaviour
{
    [SerializeField] Animator _myAnim;

    public void Activate()
    {
        _myAnim.SetTrigger("Move");
    }
}
