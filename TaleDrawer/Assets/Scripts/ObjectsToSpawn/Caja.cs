using UnityEngine;

public class Caja : SpawningObject
{
    [SerializeField] LayerMask _objectMask;

    public void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(SpawnableObjectType.Caja, gameObject);
        }
        if (collision.gameObject.TryGetComponent(out SpawningObject spawningObject) && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y + .5f, _objectMask) && spawningObject.weight > 1f)
        {
            Destroy(spawningObject.gameObject);
        }

    }

    private void OnDestroy()
    {
        interactableDelegate?.Invoke(-weight,gameObject);
    }

}
