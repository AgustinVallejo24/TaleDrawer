using UnityEngine;
using System.Linq;
using System.Collections;
public class Polea_CustomNode : CustomNode
{
    [SerializeField] Polea _polea;

    [SerializeField] Character _myCharacter;
    [SerializeField] bool left;

    protected override void Start()
    {
        base.Start();


    }
    public void GetOnSubibaja()
    {

        _myCharacter.characterModel.Jump(_jumpPosition.transform.position, ConfigurePlayer);

     
    }

    public void ConfigurePlayer()
    {
        _myCharacter.characterRigidbody.linearVelocity = Vector2.zero;
        _myCharacter.transform.parent = _polea.transform;
        _myCharacter.characterView.OnLand();
        _polea.hasPlayer = true;
        _polea.platformWeight = 2;
        if (_myCharacter.GetLastPathNode() == this)
        {
            
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
        StartCoroutine(JumpCouroutine(jumpPos));
    }
    public void CliffJump(Transform jumpPos)
    {
        StartCoroutine(CliffJumpCoroutine(jumpPos));
    }

    public void SetNeghtboursBool(CustomNode node, bool value)
    {
        if (node == null) return;
        SetCanDoEvent(node, value);
    }

    public IEnumerator CliffJumpCoroutine(Transform jumpPos)
    {
        _myCharacter.SendInputToFSM(CharacterStates.Wait);
        yield return new WaitForSeconds(.3f);
        int index = 100;
        index = neighbours.FindIndex(x => x.node == _myCharacter.GetCurrentPath().First());
        if (index != 100)
        {
            Debug.LogError("Calculeelindex");
            if (!neighbours[index].canDoEvent)
            {
                _myCharacter.ClearPath();
                _myCharacter.SendInputToFSM(CharacterStates.Wait);
                yield break;
            }


        }
        _myCharacter.characterRigidbody.gravityScale = 0;
        _myCharacter.transform.parent = null;
      _polea.hasPlayer = false;
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
        index = neighbours.FindIndex(x => x.node == _myCharacter.GetCurrentPath().First());
        if (index != 100)
        {
            Debug.LogError("Calculeelindex");
            if (!neighbours[index].canDoEvent)
            {
                _myCharacter.ClearPath();
                _myCharacter.SendInputToFSM(CharacterStates.Wait);
                yield break;
            }


        }
        _myCharacter.transform.parent = null;
        _polea.hasPlayer = false;
        _myCharacter.Jump(jumpPos);
    }
}
