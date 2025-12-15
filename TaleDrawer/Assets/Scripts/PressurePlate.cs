using UnityEngine;
using System.Collections.Generic;
public class PressurePlate : MonoBehaviour
{
    [SerializeField] List<GameObject> _currentObjects;
    [SerializeField] Animator _animator;
    [SerializeField] Trap _trap;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Character character) && !_currentObjects.Contains(collision.gameObject))
        {
            character.transform.position += Vector3.up * .29f + Vector3.right * .3f * character.flipSign;
        }
        if (_currentObjects.Count == 0 && !collision.isTrigger)
        {
            if (collision.TryGetComponent(out Rigidbody2D rb) && rb.mass > .5f)
            {
                _currentObjects.Add(collision.gameObject);
                _animator.SetTrigger("Press");
                _trap.ShootProjectile();

            }
        }
        else
        {
            _currentObjects.Add(collision.gameObject);
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent(out Character character) && !_currentObjects.Contains(collision.gameObject))
    //    {
    //        character.transform.position += Vector3.up * .29f + Vector3.right * .3f * character.flipSign;
    //    }
    //    if (_currentObjects.Count == 0 && !collision.collider.isTrigger)
    //    {
    //        if (collision.gameObject.TryGetComponent(out Rigidbody2D rb) && rb.mass > .5f)
    //        {
    //            _currentObjects.Add(collision.gameObject);
    //            _animator.SetTrigger("Press");
    //            _trap.ShootProjectile();

    //        }
    //    }
    //    else
    //    {
    //        _currentObjects.Add(collision.gameObject);
    //    }

    //}
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_currentObjects.Contains(collision.gameObject))
        {
            _currentObjects.Remove(collision.gameObject);
            if (_currentObjects.Count == 0)
            {

                _animator.SetTrigger("Release");
            }

        }

    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (_currentObjects.Contains(collision.gameObject))
    //    {
    //        Debug.LogError("Salgo");
    //        _currentObjects.Remove(collision.gameObject);
    //        if (_currentObjects.Count == 0)
    //        {

    //            _animator.SetTrigger("Release");
    //        }

    //    }

    //}
}
