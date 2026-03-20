using Unity.VisualScripting;
using UnityEngine;

public class MonkeyRenderer : MonoBehaviour
{
    [SerializeField] Monkey _myMonkey;
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _myRend;
    [SerializeField] Collider2D _attackLeftColl;
    [SerializeField] Collider2D _attackRightColl;

    public void Attack()
    {
        _myMonkey.Attack();
    }

    public void EnableAttackColliders()
    {
        if (_myRend.flipX)
        {
            if (_attackLeftColl != null)
            {
                _attackLeftColl.enabled = true;
            }
        }
        else
        {
            if (_attackRightColl != null)
            {
                _attackRightColl.enabled = true;
            }
        }
    }

    public void DisableAttackColliders()
    {
        if (_attackLeftColl != null)
        {
            _attackLeftColl.enabled = false;
        }

        if (_attackRightColl != null)
        {
            _attackRightColl.enabled = false;
        }
    }

    public void PlaySound(SoundsType type)
    {
        SoundManager.Play(type, _myMonkey.transform.position);
    }
}
