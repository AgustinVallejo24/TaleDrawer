using UnityEngine;

public class Caja : SpawningObject
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(SpawnableObjectType.Caja, gameObject);
        }
    }
}
