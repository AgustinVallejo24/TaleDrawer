using UnityEngine;

public class InterstPoint_Jump : InterestPoint
{
    [SerializeField] Transform _jumpPosition;
    protected override void Start()
    {
        base.Start();
        pointEvent.AddListener(Jump);
    }

    public void Jump()
    {
        Debug.Log("Llego al salto");
        Vector2 jumpPos = new Vector2(_jumpPosition.transform.position.x, _jumpPosition.transform.position.y);
        Character.instance.characterModel.Jump(jumpPos,1);
    }

}
