using UnityEngine;

public class PoleyObject : MonoBehaviour, IDeletable
{
    [SerializeField] Polea polea;
    
    public void Delete()
    {
        polea.netWeight = 0;
        polea.CheckWeight();
        Activation(false);
    }

    public void Activation(bool value)
    {
        GetComponent<SpriteRenderer>().enabled = value;
        GetComponent<Collider2D>().enabled = value;
    }
}
