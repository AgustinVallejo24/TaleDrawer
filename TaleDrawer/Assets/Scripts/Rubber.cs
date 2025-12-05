using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Rubber : MonoBehaviour
{
    private RectTransform rect;
    private Canvas canvas;
    public bool isMoving;
    public LayerMask spawningOwbjects;
    private Vector2 initialPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        initialPos = rect.anchoredPosition;
    }

    // Llamado desde tu Input Action (Drag / Pointer Position)
    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || isMoving) return;

        Vector2 screenPos = ctx.ReadValue<Vector2>();
        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out localPos
        );

        rect.anchoredPosition = localPos;
    }

    // Llamado desde Pointer up
    public void OnRelease()
    {
        Vector3 pos = GameManager.instance._sceneCamera.ScreenToWorldPoint(transform.position);
        var interactionHit = Physics2D.OverlapCircle(pos, 1f, spawningOwbjects);

        if (interactionHit != null && interactionHit.gameObject.TryGetComponent(out IDeletable sP))
        {
            sP.Delete();
        }
        BackToPosition();
    }

    public void BackToPosition()
    {
        isMoving = true;

        rect.DOAnchorPos(initialPos, 1f)
            .OnComplete(() => isMoving = false);
    }
}

