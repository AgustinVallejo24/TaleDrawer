using UnityEngine;

public class PoleyObject : SpawningObject
{
    [SerializeField] Polea polea;
    
    public override void Delete()
    {
        base.Delete();
        polea.netWeight = 0;
        polea.CheckWeight();
        Activation(false);
    }

    public void Activation(bool value)
    {
        GetComponent<SpriteRenderer>().enabled = value;
        if (value) SetTransparency(Color.red, 1);
        GetComponent<Collider2D>().enabled = value;
    }
}
