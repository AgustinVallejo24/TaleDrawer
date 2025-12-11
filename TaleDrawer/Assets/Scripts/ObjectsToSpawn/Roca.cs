using UnityEngine;

public class Roca : SpawningObject
{
    [SerializeField] SpawnableObjectType _objectsToDestroy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Paint()
    {
        StartCoroutine(GetComponentInChildren<Paint>().PaintSprite());
    }

    public override void Delete()
    {
        base.Delete();
        Destroy(gameObject);
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (_spawned)
        {
            if (collision.gameObject.TryGetComponent(out SpawningObject sObject) && _objectsToDestroy.HasFlag(sObject.myType))
            {
                Destroy(sObject.gameObject);
            }
        }
    }
}
