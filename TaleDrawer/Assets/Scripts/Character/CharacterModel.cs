using UnityEngine;

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
     
        Vector2 direction = objective - new Vector2(_myCharacter.transform.position.x, _myCharacter.transform.position.y);
        _myRigidbody.MovePosition(new Vector2(_myCharacter.transform.position.x, _myCharacter.transform.position.y) +  _myCharacter.currentSpeed * direction.normalized * Time.deltaTime);
    }
}
