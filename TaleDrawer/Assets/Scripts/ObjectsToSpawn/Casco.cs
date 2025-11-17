using UnityEngine;

public class Casco : SpawningObject
{
    [SerializeField] int _health;
    [SerializeField] Quaternion neededRot;


    protected override void InteractionWithPlayer()
    {
        if(Character.instance.TryGetComponent(out Robin rob))
        {
            rob.PutOnHelmet(transform, neededRot);
        }
    }
    
}
