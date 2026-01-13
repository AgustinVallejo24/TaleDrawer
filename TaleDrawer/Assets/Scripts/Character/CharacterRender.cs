using System.Collections;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class CharacterRender : MonoBehaviour
{
    [SerializeField] Character _character;
    [SerializeField] Animator _animator;
    Vector3 movimientoRoot;
    void Start()
    {


        // Si usas un Animator, es buena práctica desactivar
        // la actualización automática de la posición para evitar conflictos.
        // _animator.applyRootMotion = true; // (Generalmente solo para 3D Humanoid, pero a veces funciona en 2D si hay un root bone)
    }

    public void TPPlayer()
    {

    }
    public void TPChild()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer.flipX)
        {
            _character.transform.position = _character.transform.position + 1.5f * Vector3.left + 1.5f * Vector3.up;
        }
        else
        {
            _character.transform.position = _character.transform.position + 1.5f * Vector3.right + 1.5f * Vector3.up;
        }
        _character.characterRigidbody.gravityScale = 3;
        _character.SendInputToFSM(CharacterStates.Idle);
        transform.localPosition = new Vector3(0, 0, 0);
        _character.climbAction?.Invoke();
    }

    public void PullLever()
    {
        _character.currentLever.ActivateLever();
    }
    public void GoToIdle()
    {
        _character.SendInputToFSM(CharacterStates.Idle);
    }

    public void GoToMoving()
    {
        _character.SendInputToFSM(CharacterStates.Moving);
    }

    public void ExitRope()
    {
        StartCoroutine(IExitRope());
    }

    public void CurrentRopeAnimationStatus(int value)
    {
        _character.currentHook.RopeAnimationManager(value);
    }
    public void Jump()
    {
        //float distance = Vector2.Distance(_character.transform.position, _character.currentJumpingPosition);
        //_character.transform.DOJump(_character.currentJumpingPosition, 0.5f * distance, 1, _character.currentJumpingTime)
        //.SetEase(Ease.Linear)
        //.OnComplete(() =>
        //{
        //   _character.currentJumpingAction?.Invoke();
        //});
        _character.characterModel.Jump();
    }
    private IEnumerator IExitRope()
    {
        if (_character.currentHook.myType == RopeType.Vertical)
        {
            _character.transform.position = _character.currentHook.rope.firstPoint.position;
            transform.position = new Vector3(0, 0, 0);

            yield return new WaitForSeconds(0.2f);

            _character.SendInputToFSM(CharacterStates.Moving);
            _character.currentHook = null;
        }
    }
    public void StopRigidbody()
    {
        _character.characterRigidbody.linearVelocity = Vector2.zero;
    }
    public void RestoreSpeed()
    {
        _character.currentSpeed = _character.maxSpeed;
    }
    public void ExitingHorizontalRope()
    {
        _character.currentHook.ExitingHorizontalRope(transform);
    }
}
