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
            _myCharacter.characterModel.Jump(subibaja.sides[0].position);
        }
        else if (!subibaja.left && !left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[1].position);
        }
    }

    public void NewJump(Transform jumpPos)
    {
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
