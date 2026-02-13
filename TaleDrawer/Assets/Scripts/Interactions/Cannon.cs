using UnityEngine;

public class Cannon : MonoBehaviour, IInteractableSP
{
    [SerializeField] Character _character;
    [SerializeField] Collider2D _playerDetectionCollider;
    [SerializeField] SpawningObject _soga;
    [SerializeField] Animator animator;
    [SerializeField] Transform _shootingPos;
    [SerializeField] CannonBall _cannonBall;
    [SerializeField] int _playerVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out Character character))
        {
            Collider2D myCollider = GetComponent<Collider2D>();

            ColliderDistance2D distance = myCollider.Distance(collision);

            Vector2 normal = distance.normal;
            float angle = Mathf.Abs(Vector2.Angle(normal, Vector2.up));
            Debug.LogError(angle);
            if (angle < 100)
            {
               
                _character.SendInputToFSM(CharacterStates.DoingEvent);
                _character.SetAnimatorTrigger("PullRope");
          
                character.characterRigidbody.gravityScale = 0;
                character.characterRigidbody.linearVelocity = Vector2.zero;
                animator.SetTrigger("Pull");
                _soga.myAnim.SetTrigger("Pull");
                if (_character.transform.position.x >= _soga.transform.position.x)
                {
                    _playerVelocity = 1;
                    _character.transform.position = new Vector3(_soga.transform.position.x + .8f, _character.transform.position.y, 0);
                }
                else
                {
                    _playerVelocity = -1;
                    _character.transform.position = new Vector3(_soga.transform.position.x - .8f, _character.transform.position.y, 0);
                }
            }


        }
    }

    public void Shoot()
    {
      CannonBall cannonBall =  Instantiate(_cannonBall);
        cannonBall.transform.position = _shootingPos.position;
        cannonBall.transform.right = _shootingPos.right;
    }
    public void ReleasePlayer()
    {
        _character.characterRigidbody.gravityScale = 3;
        _character.characterRigidbody.AddForce(_playerVelocity *3 * Vector2.right + 4 * Vector2.up,ForceMode2D.Impulse);
        StartCoroutine(_character.SendInputToFSM(CharacterStates.Idle,.5f));

    }
    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
     
    }

    public void Interact(SpawningObject spawningObject)
    {
        if (spawningObject.myType == SpawnableObjectType.Soga)
        {

            _playerDetectionCollider.enabled = true;


            GameManager.instance.RemoveSpawningObjectFromList(spawningObject);

            Destroy(spawningObject.gameObject);


            _soga.gameObject.SetActive(true);
            _soga.SetTransparency(Color.red, 1);


        }
        else
        {
            spawningObject.CantInteract();
        }

    }

    public void Interact(GameObject interactor)
    {
        
    }

    public void InteractWithPlayer()
    {
       
    }

    public void InsideInteraction()
    {
       
    }

    public InteractableType MyInteractableType()
    {
        throw new System.NotImplementedException();
    }
}
