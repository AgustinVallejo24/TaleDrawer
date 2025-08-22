using UnityEngine;
using DG.Tweening;
public class CharacterModel
{
    Rigidbody2D _myRigidbody;
    Character _myCharacter;
    public CharacterModel(Character character, Rigidbody2D rigidbody)
    {
        _myRigidbody = rigidbody;
        _myCharacter = character;
    }

    public virtual void Move(Vector2 objective)
    {
        _myRigidbody.linearVelocity = Vector2.ClampMagnitude(_myRigidbody.linearVelocity, 4);
        Vector2 direction = objective - new Vector2(_myCharacter.transform.position.x, _myCharacter.transform.position.y);
        _myRigidbody.AddForce(_myCharacter.currentSpeed * direction.normalized * Time.deltaTime,ForceMode2D.Impulse);
    }

    public void Jump(Vector2 jumpPosition,float duration)
    {
        _myCharacter.SendInputToFSM(CharacterStates.Jumping);
        Vector2 direction = (jumpPosition - CustomTools.ToVector2(_myCharacter.transform.position)).normalized * _myCharacter.currentJumpForce.x + Vector2.up * _myCharacter.currentJumpForce.y;
        _myRigidbody.AddForce(direction , ForceMode2D.Impulse);
    }
}
