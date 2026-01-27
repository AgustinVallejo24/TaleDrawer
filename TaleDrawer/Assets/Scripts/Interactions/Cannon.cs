using UnityEngine;

public class Cannon : MonoBehaviour, IInteractable
{
    [SerializeField] Character _character;
    [SerializeField] Collider2D _playerDetectionCollider;
    [SerializeField] SpawningObject _soga;
    [SerializeField] Animator animator;
    [SerializeField] Transform _shootingPos;
    [SerializeField] CannonBall _cannonBall;
    Vector2 _playerVelocity;
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
            _character.SendInputToFSM(CharacterStates.DoingEvent);
            _character.SetAnimatorTrigger("PullRope");
            character.characterRigidbody.gravityScale = 0;
            character.characterRigidbody.linearVelocity = Vector2.zero;
            animator.SetTrigger("Pull");
            _soga.myAnim.SetTrigger("Pull");

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
        _character.characterRigidbody.AddForce(Mathf.Sign(_playerVelocity.x) * 3 * Vector2.right + 3 * Vector2.up,ForceMode2D.Impulse);
        _character.SendInputToFSM(CharacterStates.Moving);

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
