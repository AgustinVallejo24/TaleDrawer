using UnityEngine;

public class Robin : Character
{

    
    protected override void Awake()
    {
        characterModel = new CharacterModel(this, _characterRigidbody);
        characterView = new CharacterView(this, _animator, _characterSprite);
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.TryGetComponent(out Subibaja subibaja))
        {
            if (subibaja.isMoving)
            {
                Debug.LogError("entro");
                _characterRigidbody.linearVelocity = Vector2.zero;
                transform.position = subibaja.tpPoint.position;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
                SendInputToFSM(CharacterStates.Stop);
            }
            else
            {
                Debug.LogError("aca tambien");
                subibaja.myCharacter = this;
                _characterRigidbody.linearVelocity = Vector2.zero;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, subibaja.transform.rotation.z, transform.rotation.w);
                transform.parent = subibaja.transform;

            }
        }
    }

}
