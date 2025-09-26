using UnityEngine;
using DG.Tweening;
public class CharacterModel
{
    Rigidbody2D _myRigidbody;
    Character _myCharacter;
    Vector2 speed = Vector2.zero;
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
    public void Jump(Vector2 jumpPosition,float horizontalJumpForce, float verticalJumpForce)
    {
        _myCharacter.SendInputToFSM(CharacterStates.Jumping);
        Vector2 direction = (jumpPosition - CustomTools.ToVector2(_myCharacter.transform.position)).normalized * horizontalJumpForce + Vector2.up * verticalJumpForce;
        _myRigidbody.AddForce(direction, ForceMode2D.Impulse);
    }
    public virtual void Move2(Vector2 objective, float smoothSpeed)
    {
        Vector2 direction = objective - new Vector2(_myCharacter.transform.position.x, _myCharacter.transform.position.y);
        //_myRigidbody.linearVelocity = Vector2.ClampMagnitude(direction.normalized * _myCharacter.currentSpeed * 25 * Time.deltaTime, 4);        
        //_myRigidbody.linearVelocity = Vector2.SmoothDamp(_myRigidbody.linearVelocity, direction.normalized, ref speed, smoothSpeed);
        _myRigidbody.linearVelocity = Vector2.SmoothDamp(_myRigidbody.linearVelocity,
            Vector2.ClampMagnitude(direction.normalized * _myCharacter.currentSpeed * Time.deltaTime, 25), ref speed, smoothSpeed);
    }

}
