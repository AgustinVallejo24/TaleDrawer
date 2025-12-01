using System.Collections;
using UnityEngine;
using System.Linq;
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
            _character.transform.position = _character.transform.position + 1.5f * Vector3.left + 2 * Vector3.up;
        }
        else
        {
            _character.transform.position = _character.transform.position + 1.5f * Vector3.right + 2 * Vector3.up;
        }
        
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

    private IEnumerator IExitRope()
    {
        _character.transform.position = transform.position;
        transform.position = _character.transform.position;
        _character.transform.position = _character._currentPath.First().transform.position;

        yield return new WaitForSeconds(0.2f);

        _character.SendInputToFSM(CharacterStates.Moving);
    }
}
