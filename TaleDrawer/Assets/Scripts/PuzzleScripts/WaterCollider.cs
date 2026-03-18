using UnityEngine;
public class WaterCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ParticleSystem _SplashSystem;
    [SerializeField] ParticleSystem _WalkingParticles;
    [SerializeField] ParticleSystem _WalkingParticlesSurface;
    [SerializeField] ParticleSystem _WalkingSplashParticles;
    [SerializeField] Character character1;
    float timer = .5f;
    [SerializeField] GameObject liquidPrefab;
    [SerializeField] MetaballManager manager;
    [SerializeField] float dragForce;
     BoxCollider2D collider2D;
    void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (character1 != null)
        {

            if(character1.entityRigidbody.linearVelocityX != 0)
            {
                if (!_WalkingParticles.isPlaying)
                {
                    _WalkingParticles.Play();
                }
                if (!_WalkingParticlesSurface.isPlaying)
                {
                    _WalkingParticlesSurface.Play();
                }
                if (!_WalkingSplashParticles.isPlaying)
                {
                    _WalkingSplashParticles.Play();
                }

                _WalkingParticles.transform.position = new Vector3(character1.transform.position.x, collider2D.bounds.center.y, 0);

                _WalkingParticlesSurface.transform.position = new Vector3(character1.transform.position.x +character1.flipSign * .3f, collider2D.bounds.max.y+.1f, 0);
                _WalkingSplashParticles.transform.position = new Vector3(character1.transform.position.x + character1.flipSign*.1f, collider2D.bounds.max.y+.1f, 0);


            }
            else
            {
                _WalkingParticles.Stop();
                _WalkingParticlesSurface.Stop();   
                _WalkingSplashParticles.Stop();
            }
        
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Character>(out Character character))
        {
            character.currentSpeed /=  2;
            character.entityRigidbody.gravityScale /= 2;
            character.animator.speed = .5f;
            character.jumpForce /= 1.5f;
            ColliderDistance2D dist = collision.Distance(GetComponent<Collider2D>());
            Vector2 punto = dist.pointA;
            var spS = Instantiate(_SplashSystem, transform);
            spS.transform.position = punto;
            character1 = character;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character>(out Character character))
        {
            character.currentSpeed = character.maxSpeed;
            character.entityRigidbody.gravityScale = 3;
            character.animator.speed = 1f;
            character.jumpForce *= 1.5f;
            ColliderDistance2D dist = collision.Distance(GetComponent<Collider2D>());
            Vector2 punto = dist.pointA;
            var spS = Instantiate(_SplashSystem, transform);
            spS.transform.position = punto;
            character1 = null;
            _WalkingParticles.Stop();
            _WalkingParticlesSurface.Stop();
            _WalkingSplashParticles.Stop();
        }

    }
}
