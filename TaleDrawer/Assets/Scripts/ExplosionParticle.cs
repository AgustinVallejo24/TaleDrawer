using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem _base;
    [SerializeField] ParticleSystem _burst;

    public void Start()
    {
        Destroy(gameObject, 3);
    }
    public void Play()
    {
        _base.Play();
        _burst.Play();
    }
}
