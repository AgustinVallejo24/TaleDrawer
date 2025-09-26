using UnityEngine;

public class SubibajaEn_InterestPoint : InterestPoint
{
    [SerializeField] Character _myCharacter;
    [SerializeField] Subibaja subibaja;
    [SerializeField] bool left;


    protected override void Start()
    {
        base.Start();

        _myCharacter = Character.instance;
    }
    public void GetOnSubibaja()
    {
        if (subibaja.left && left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[0].position,5f,5f);
        }
        else if (!subibaja.left && !left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[1].position, 5f, 5f);
        }
    }

    public void GetOnSubibaja(string side)
    {

    }
}
