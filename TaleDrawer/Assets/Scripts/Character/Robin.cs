using UnityEngine;
using DG.Tweening;
public class Robin : Character
{
    public Sequence currentTween;
    public Subibaja subibaja;
    public bool canClimb;
    protected override void Awake()
    {
        characterModel = new CharacterModel(this, characterRigidbody);
        characterView = new CharacterView(this, _animator, _characterSprite);
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

    }

    public override void Update()
    {
        base.Update();

        
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.TryGetComponent(out Subibaja newSubibaja))
        {
            
            if (newSubibaja.isMoving && _currentState != CharacterStates.Wait)
            {
                Debug.LogError("entro");
                characterRigidbody.linearVelocity = Vector2.zero;
                DOTween.Kill(transform);
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
                SendInputToFSM(CharacterStates.Idle);
                
            }

        }      
        
        if(collision.TryGetComponent(out IInteractable inter) && inter.MyInteractableType() == InteractableType.ClimbingObj)
        {
            canClimb = true;
            currentInteractable = inter;
            
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable inter) && inter.MyInteractableType() == InteractableType.ClimbingObj)
        {
            canClimb = false;
            currentInteractable = null;
        }
    }


}
