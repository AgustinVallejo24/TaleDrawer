using UnityEngine;
using System.Linq;
public class Subibaje_CustomNode : CustomNode
{
    [SerializeField] Subibaja _subibaja;

    [SerializeField] Character _myCharacter;
    [SerializeField] Subibaja subibaja;
    [SerializeField] bool left;

    private RelativeJoint2D joint;
    protected override void Start()
    {
        base.Start();
        goalDelegate = subibaja.CreateJoint;
     
    }
    public void GetOnSubibaja()
    {
   
        if (left && subibaja.left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[0].position, ConfigurePlayer);
        }
        else if (left && !subibaja.left)
        {
            _myCharacter.ClearPath();
            _myCharacter.characterRigidbody.linearVelocity = Vector2.zero;
            _myCharacter.SendInputToFSM(CharacterStates.Idle);
        }
        else if(!left && !subibaja.left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[1].position, ConfigurePlayer);
        }
        else
        {
            _myCharacter.ClearPath();
            _myCharacter.characterRigidbody.linearVelocity = Vector2.zero;
            _myCharacter.SendInputToFSM(CharacterStates.Idle);
        }
    }
    public void GetOnSubibajaFromHeaven()
    {
        _myCharacter.characterModel.Jump(subibaja.sides[0].position, ConfigurePlayer);
    }
    public void ConfigurePlayer()
    {


        _myCharacter.characterView.OnLand();
        _myCharacter.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, subibaja.transform.rotation.z, transform.rotation.w);
        _myCharacter.transform.parent = subibaja.transform;
        //   _myCharacter.characterRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
       // subibaja.CreateJoint(_myCharacter.GetComponent<Rigidbody2D>());

        subibaja.hasPlayer = true;
        if (_myCharacter.GetLastPathNode() == this)
        {
            goalDelegate?.Invoke();
            _myCharacter.ClearPath();
            StartCoroutine(_myCharacter.SendInputToFSM(CharacterStates.Wait, 0.2f));
        }
        else
        {
            StartCoroutine(_myCharacter.SendInputToFSM(CharacterStates.Moving, 0.2f));
        }
        

    }
    public void NewJump(Transform jumpPos)
    {
        int index = 100;
        index = neighbours.FindIndex(x => x.node == _myCharacter.GetCurrentPath().First());
        if (index != 100)
        {
            Debug.LogError("Calculeelindex");
            if (!neighbours[index].canDoEvent)
            {
                _myCharacter.ClearPath();
                _myCharacter.SendInputToFSM(CharacterStates.Wait);
                return;
            }
            

        }
        _myCharacter.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
        _myCharacter.transform.parent = null;
        Destroy(subibaja.myJoint);
        subibaja.hasPlayer = false;
        _myCharacter.Jump(jumpPos);
    }
    public void SetNeghtboursBool(CustomNode node,bool value)
    {
        if (node == null) return;
         SetCanDoEvent(node, value);
    }

}
