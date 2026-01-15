using UnityEngine;
using DG.Tweening;
public class Robin : Character
{
    public Sequence currentTween;
    public Subibaja subibaja;
    public bool canClimb;
    protected override void Awake()
    {
        characterModel = new CharacterModel(this, characterRigidbody,floorLayerMask);
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
   
        
        if(collision.TryGetComponent(out IInteractable inter) && inter.MyInteractableType() == InteractableType.ClimbingObj)
        {
            canClimb = true;
            currentInteractable = inter;
            
        }

    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (collision.TryGetComponent(out IInteractable inter) && inter.MyInteractableType() == InteractableType.ClimbingObj)
        {
            canClimb = false;
            currentInteractable = null;
        }
    }


}
