using UnityEngine;
using System.Collections.Generic;
public class PressurePlate : MonoBehaviour
{
    [SerializeField] List<GameObject> _currentObjects;
    [SerializeField] Animator _animator;
    [SerializeField] Trap _trap;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collider2D myCollider = GetComponent<Collider2D>();

        ColliderDistance2D distance = myCollider.Distance(collision);

        Vector2 normal = distance.normal;
        float angle = Mathf.Abs(Vector2.Angle(normal, Vector2.up));
        Debug.LogError(angle);
        if (collision.gameObject.TryGetComponent(out Character character) && !_currentObjects.Contains(collision.gameObject) && angle > 10)
        {
           character.transform.position += Vector3.up * .26f + Vector3.right * .3f * character.flipSign;
            Debug.LogError("Entro aca y la velocit es: " + character.characterRigidbody.linearVelocityY);
        }
        if (_currentObjects.Count == 0 && !collision.isTrigger)
        {
            if (collision.TryGetComponent(out Rigidbody2D rb) && rb.mass > .5f)
            {
                rb.linearVelocityY = 0;
                _currentObjects.Add(collision.gameObject);
                _animator.SetTrigger("Press");
                _trap.Activation();

            }
        }
        else
        {
            if(!_currentObjects.Contains(collision.gameObject) && !collision.isTrigger)
            _currentObjects.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_currentObjects.Contains(collision.gameObject))
        {
            _currentObjects.Remove(collision.gameObject);
            if (_currentObjects.Count == 0)
            {

                _animator.SetTrigger("Release");
                _trap.Deactivation();
            }

        }

    }

}
