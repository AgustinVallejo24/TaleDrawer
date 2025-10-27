using UnityEngine;
using DG.Tweening;
public class CharacterModel
{
    Rigidbody2D _myRigidbody;
    Character _myCharacter;
    Vector2 speed = Vector2.zero;
    private Vector2 _smoothDampVelocity;
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

    //public void Jump(Vector2 jumpPosition)
    //{
    //    _myCharacter.SendInputToFSM(CharacterStates.Jumping);
    //    Vector2 direction = (jumpPosition - CustomTools.ToVector2(_myCharacter.transform.position)).normalized * _myCharacter.currentJumpForce.x + Vector2.up * _myCharacter.currentJumpForce.y;
    //    _myRigidbody.AddForce(direction , ForceMode2D.Impulse);
    //}

    public void Jump(Vector2 jumpPosition,float horizontalJumpForce, float verticalJumpForce)
    {
        _myCharacter.SendInputToFSM(CharacterStates.Jumping);
        Vector2 direction = (jumpPosition - CustomTools.ToVector2(_myCharacter.transform.position)).normalized * horizontalJumpForce + Vector2.up * verticalJumpForce;
        _myRigidbody.AddForce(direction, ForceMode2D.Impulse);
    }
    public void Jump(Vector2 jumpPosition)
    {
        float distance = Vector2.Distance(_myCharacter.transform.position, jumpPosition);
        _myCharacter.SendInputToFSM(CharacterStates.Jumping);
        _myCharacter.transform.DOJump(jumpPosition, .5f * distance, 1, 1
            ).SetEase(Ease.Linear);

    }
    public virtual void Move2(Vector2 objective, float smoothSpeed)
    {
        Vector2 currentPosition = _myRigidbody.position; // Usar Rigidbody.position para f�sica
        Vector2 direction = objective - currentPosition;
        float distance = direction.magnitude;
        float stopThreshold = 0.1f; // Distancia m�nima para considerar que ya llegamos

        if (distance <= stopThreshold)
        {
            // Si estamos cerca, detenemos el personaje inmediatamente y salimos.
            _myRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        // Calcular la velocidad objetivo (sin Time.deltaTime)
        // Usar la distancia para escalar la velocidad si estamos en la zona de frenado
        float maxSpeed = _myCharacter.currentSpeed;

        // Opcional: Reducir la velocidad linealmente cuando est� cerca del destino
        // para un frenado m�s controlado, aunque SmoothDamp ya lo hace.
        // float targetSpeed = Mathf.Min(maxSpeed, maxSpeed * (distance / 2f)); // Ejemplo de rampa de frenado
        float targetSpeed = maxSpeed;

        Vector2 targetVelocity = direction.normalized * targetSpeed;

        // Aplicar SmoothDamp a la velocidad del Rigidbody
        // **Importante:** Usamos _myRigidbody.velocity, no linearVelocity, 
        // y la variable 'speed' debe ser un Vector2 pasado por ref, no un float.
        Vector2 currentVelocity = _myRigidbody.linearVelocity;
        //Vector2 smoothVelocity; // Variable para la velocidad actual (ref) del SmoothDamp.

        // Necesitas una variable de tipo Vector2 para el ref. 
        // Decl�rala a nivel de clase/componente: private Vector2 _smoothDampVelocity;

        _myRigidbody.linearVelocity = Vector2.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref _smoothDampVelocity, // Usa la variable de clase (ref)
            smoothSpeed
        );
    }

}
