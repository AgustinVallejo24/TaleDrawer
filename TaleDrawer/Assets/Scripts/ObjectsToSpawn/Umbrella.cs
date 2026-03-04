using UnityEngine;

public class Umbrella : SpawningObject
{
    [SerializeField] Paint pintsSc;
    public override void InteractionWithEntity()
    {
        Destroy(gameObject);
    }
}
