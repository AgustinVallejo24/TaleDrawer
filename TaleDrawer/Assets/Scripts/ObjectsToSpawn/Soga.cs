using UnityEngine;
using UnityEngine.UI;

public class Soga : SpawningObject, IInteractableP
{
    [SerializeField] Hook hook;
    public Transform firstPoint;
    public Transform secondPoint;
    public RopeType myRopeType;
    public SpriteRenderer mySpRenderer;
    [SerializeField] InteractableType _interactableType;
    [SerializeField] Collider2D _detectionCollider;
    [SerializeField] Collider2D _fallingCollider;
    [SerializeField] GameObject _eKey;

    public void OnErased()
    {
        gameObject.SetActive(false);
    }
    public override void Paint()
    {
        StartCoroutine(GetComponentInChildren<Paint>().PaintSprite());
    }
    private void Update()
    {
        if(myRopeType == RopeType.Vertical && hook != null)
        {
            hook.RopeAnimatorSpeedController();
        }

    }

    public override void Delete()
    {
        base.Delete();

        hook.Delete();
    }

    public override void OnSpawned()
    {
        base.OnSpawned();
        _fallingCollider.enabled = true;
        _detectionCollider.enabled = false;
    }

    public void Interact()
    {
        if (hook != null)
        {
            hook.InteractWithPlayer();
        }
    }


    public InteractableType MyInteractableType()
    {
        return InteractableType.ClimbingObj;
    }
    public KeyCode InteractionKey()
    {
        return KeyCode.E;
    }
    public void OnLeavingInteraction()
    {
        throw new System.NotImplementedException();
    }
}
