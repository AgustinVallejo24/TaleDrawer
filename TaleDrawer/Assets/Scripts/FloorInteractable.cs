using UnityEngine;

public class FloorInteractable : MonoBehaviour, IInteractable
{

    [SerializeField] FloorTrap trap;
    Character _myCharacter;
    [SerializeField] Transform _jumpPos;
    public void InsideInteraction()
    {

    }

    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {

    }

    public void Interact(SpawningObject spawningObject)
    {

    }

    public void Interact(GameObject interactor)
    {

    }

    public void InteractWithPlayer()
    {
        
        if (trap.open)
        {
            _myCharacter = Character.instance;
            _myCharacter.GetPath(trap.GetClosestOuterNode(),trap.GetClosestJumpPos().position);
            _myCharacter.SendInputToFSM(CharacterStates.Moving);
            _myCharacter.onMovingEnd = PlayerJump;
        }
    }
    
    public void PlayerJump()
    {
        _myCharacter.onMovingEnd = null;
        if (trap.open)
        {
            _myCharacter.characterModel.Jump(_jumpPos.position, () =>
            {

                _myCharacter.Land();

            });
        }
        else
        {
            _myCharacter.ClearPath();
            _myCharacter.characterRigidbody.linearVelocity = Vector2.zero;
            _myCharacter.SendInputToFSM(CharacterStates.Idle);
        }

    }
}
