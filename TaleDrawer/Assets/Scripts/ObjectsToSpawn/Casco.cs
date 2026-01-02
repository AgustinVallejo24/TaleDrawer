using UnityEngine;

public class Casco : SpawningObject
{
    [SerializeField] public int _health;
    [SerializeField] public Quaternion neededRot;


    public override void InteractionWithEntity()
    {
        if(Character.instance.helmet == null)
        {
            Character.instance.PutOnHelmet(this, _myrb);
        }
    }
    
}
