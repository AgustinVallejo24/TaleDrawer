using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] GameObject _currentObject;
    [SerializeField] Animator _animator;
    [SerializeField] Trap _trap;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_currentObject == null)
        {
            if(collision.TryGetComponent(out Rigidbody2D rb) && rb.mass>.5f)
            {
                _currentObject = collision.gameObject;
                _animator.SetTrigger("Press");
                _trap.ShootProjectile();
              
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject == _currentObject)
        {
            _currentObject = null;
            _animator.SetTrigger("Release");
        }
    }
}
