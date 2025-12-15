using UnityEngine;
using DG.Tweening;
using System;
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


    public void Jump(Vector2 jumpPosition,float horizontalJumpForce, float verticalJumpForce)
    {
        _myCharacter.SendInputToFSM(CharacterStates.Jumping);
        Vector2 direction = (jumpPosition - CustomTools.ToVector2(_myCharacter.transform.position)).normalized * horizontalJumpForce + Vector2.up * verticalJumpForce;
        _myRigidbody.AddForce(direction, ForceMode2D.Impulse);
    }
    public void Jump(Vector2 jumpPosition, Action onComplete = null, bool toJumpingState = true, float time = 1)
    {
        float distance = Vector2.Distance(_myCharacter.transform.position, jumpPosition);
        _myCharacter.characterAudioSource.clip = _myCharacter.jumpSound;
        _myCharacter.characterAudioSource.Play();
        if (toJumpingState)
        {
            _myCharacter.SendInputToFSM(CharacterStates.Jumping);
        }        

        if (Mathf.Sign(jumpPosition.x - _myCharacter.transform.position.x) > 0)
        {
            _myCharacter.characterView.FlipCharacter(1);
        }
        else
        {
            _myCharacter.characterView.FlipCharacter(-1);
        }
        _myCharacter.transform
            .DOJump(jumpPosition, 0.5f * distance, 1, time)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
            // Llamar a OnLand
            

            // Ejecutar action si vino por parámetro
            onComplete?.Invoke();
            });
    }
    public void Flip(Vector3 position)
    {
        if (Mathf.Sign(position.x - _myCharacter.transform.position.x) > 0)
        {
            _myCharacter.characterView.FlipCharacter(1);
        }
        else
        {
            _myCharacter.characterView.FlipCharacter(-1);
        }
    }
    public virtual void Move(Vector2 objective, float smoothSpeed)
    {
        Vector2 currentPosition = _myRigidbody.position; // Usar Rigidbody.position para f�sica
        Vector2 direction = new Vector2(objective.x,_myCharacter.transform.position.y) - currentPosition;
       // float distance = direction.magnitude;
      //  float stopThreshold = 0.1f; // Distancia m�nima para considerar que ya llegamos

        //if (distance <= stopThreshold)
        //{

        //    Debug.LogError("LACONCHA");
        //    // Si estamos cerca, detenemos el personaje inmediatamente y salimos.
        //    _myRigidbody.linearVelocity = Vector2.zero;
        //    return;
        //}

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

        RaycastHit2D hit = Physics2D.Raycast(_myRigidbody.position + targetVelocity.normalized * .2f, Vector2.down, 2f, _myCharacter.floorLayerMask);

        if (hit)
        {
      //      Debug.LogError(hit.transform.gameObject.name);
            Vector2 tangent = Vector2.Perpendicular(hit.normal).normalized;

            if (Vector2.Dot(tangent, direction) < 0)
               tangent = -tangent;

            targetVelocity = tangent * targetSpeed;
        }
        //Vector2 smoothVelocity; // Variable para la velocidad actual (ref) del SmoothDamp.
        Debug.DrawLine(_myCharacter.transform.position, CustomTools.ToVector2(_myCharacter.transform.position) + Vector2.down);
        Debug.DrawLine(_myCharacter.transform.position, CustomTools.ToVector2(_myCharacter.transform.position) + targetVelocity);
        // Necesitas una variable de tipo Vector2 para el ref. 
        // Decl�rala a nivel de clase/componente: private Vector2 _smoothDampVelocity;

        //  _myRigidbody.MovePosition(_myRigidbody.position + direction.normalized * Time.deltaTime * targetSpeed);

        _myRigidbody.linearVelocity = Vector2.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref _smoothDampVelocity, // Usa la variable de clase (ref)
            smoothSpeed
        );
    }

}
