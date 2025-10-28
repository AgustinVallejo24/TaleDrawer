using UnityEngine;

public class Subibaje_CustomNode : CustomNode
{
    [SerializeField] Subibaja _subibaja;

    [SerializeField] Character _myCharacter;
    [SerializeField] Subibaja subibaja;
    [SerializeField] bool left;


    protected override void Start()
    {
        base.Start();

     
    }
    public void GetOnSubibaja()
    {
        if (subibaja.left && left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[0].position, ConfigurePlayer);
        }
        else if (!subibaja.left && !left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[1].position, ConfigurePlayer);
        }
    }

    public void ConfigurePlayer()
    {
        _myCharacter.ClearPath();
        _myCharacter.characterView.OnLand();
        _myCharacter.characterRigidbody.linearVelocity = Vector2.zero;
        _myCharacter.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, subibaja.transform.rotation.z, transform.rotation.w);
        _myCharacter.transform.parent = subibaja.transform;
        StartCoroutine(_myCharacter.SendInputToFSM(CharacterStates.Wait, 0.2f));

    }
    public void NewJump(Transform jumpPos)
    {
        Debug.LogError("Entro al nuevo jump");
        _myCharacter.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
        _myCharacter.transform.parent = null;
        _myCharacter.Jump(jumpPos);
    }

    public void SetNeghtboursBool(int value)
    {
        if(value == 0)
        {
            var lowerNode = neighbours[0];
            lowerNode.canDoEvent = true;
            neighbours[0] = lowerNode;
            var upperNode = neighbours[1];
            upperNode.canDoEvent = false;
            neighbours[1] = upperNode;

        }
        if (value == 1)
        {
            var lowerNode = neighbours[1];
            lowerNode.canDoEvent = true;
            neighbours[1] = lowerNode;
            var upperNode = neighbours[0];
            upperNode.canDoEvent = false;
            neighbours[0] = upperNode;
        }

    }
}
