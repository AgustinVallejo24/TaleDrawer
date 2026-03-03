using UnityEngine;

public class Boleadoras : SpawningObject
{
    public override void Paint()
    {
        StartCoroutine(GetComponentInChildren<Paint>().PaintSprite());
    }

    public override void InteractionWithEntity()
    {
        base.InteractionWithEntity();
        Destroy(gameObject);
        Character.instance.CatchBoleadoras();
    }

}
