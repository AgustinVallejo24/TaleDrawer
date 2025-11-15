using UnityEngine;

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
        Debug.LogError("PAUSA");
        _character.transform.position = _character.transform.position + 1.5f * Vector3.right + 2 * Vector3.up;
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
