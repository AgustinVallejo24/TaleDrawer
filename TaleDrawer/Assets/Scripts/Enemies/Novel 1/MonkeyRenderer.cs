using UnityEngine;

public class MonkeyRenderer : MonoBehaviour
{
    [SerializeField] Monkey _myMonkey;
    [SerializeField] Animator _animator;

    public void Attack()
    {
        _myMonkey.Attack();
    }
}
