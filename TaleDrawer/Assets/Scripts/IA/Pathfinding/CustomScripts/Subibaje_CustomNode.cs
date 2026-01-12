using UnityEngine;
using System.Linq;
using System.Collections;
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
            _myCharacter.characterRigidbody.linearVelocity = Vector2.zero;
            _myCharacter.SendInputToFSM(CharacterStates.Idle);
        }
        else if(!left && !subibaja.left)
        {
            _myCharacter.characterModel.Jump(subibaja.sides[1].position, ConfigurePlayer);
        }
        else
        {
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

        

    }
    public void NewJump(Transform jumpPos)
    {
        StartCoroutine(JumpCouroutine(jumpPos));
    }
    public void CliffJump(Transform jumpPos)
    {
        StartCoroutine(CliffJumpCoroutine(jumpPos));
    }
   
    public void SetNeghtboursBool(CustomNode node,bool value)
    {
        if (node == null) return;
         SetCanDoEvent(node, value);
    }

    public IEnumerator CliffJumpCoroutine(Transform jumpPos)
    {
        _myCharacter.SendInputToFSM(CharacterStates.Wait);
        yield return new WaitForSeconds(.3f);
        int index = 100;
        if (index != 100)
        {
            Debug.LogError("Calculeelindex");
            if (!neighbours[index].canDoEvent)
            {
                _myCharacter.SendInputToFSM(CharacterStates.Wait);
                yield break;
            }


        }
        _myCharacter.transform.parent = null;
        _myCharacter.characterRigidbody.gravityScale = 0;
        Destroy(subibaja.myJoint);
        subibaja.hasPlayer = false;
        _myCharacter.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
        _myCharacter.climbAction = () =>
        {
            _myCharacter.SendInputToFSM(CharacterStates.Moving);
            _myCharacter.characterRigidbody.gravityScale = 1;
            _myCharacter.climbAction = null;
        };
        _myCharacter.characterModel.Jump(
            jumpPos.position,
            () =>
            {
                _myCharacter.SendInputToFSM(CharacterStates.Climb);
            },
            true,   // tercer parámetro: toJumpingState
            0.5f    // cuarto parámetro: time
        );
    }
    public IEnumerator JumpCouroutine(Transform jumpPos)
    {
        _myCharacter.SendInputToFSM(CharacterStates.Wait);
        yield return new WaitForSeconds(.3f);
        int index = 100;
        if (index != 100)
        {
            Debug.LogError("Calculeelindex");
            if (!neighbours[index].canDoEvent)
            {

                _myCharacter.SendInputToFSM(CharacterStates.Wait);
                yield break;
            }


        }
        _myCharacter.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
        _myCharacter.transform.parent = null;
        Destroy(subibaja.myJoint);
        subibaja.hasPlayer = false;
     }
}
