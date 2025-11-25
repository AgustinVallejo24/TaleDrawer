using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    [SerializeField] SceneManager sceneManager;
    public SpawningObject currentDraggable;
    [SerializeField] LayerMask _draggableItems;
    Vector2 _clickPosition;
    [SerializeField] InputActionReference MoveMouse;
    [SerializeField] InputActionReference Draw;
    [Header("Settings")]
    public float dragThreshold = 20f;

    private Vector2 startPos;
    private bool isDragging = false;
    private bool pointerDown = false;

    void Start()
    {
        MoveMouse.action.performed += OnDrag;
        Draw.action.performed += Drawing;
    }

    // Update is called once per frame


    private void Update()
    {
        if (!pointerDown) return;

        Vector2 currentPos = Draw.action.ReadValue<Vector2>();
        float distance = Vector2.Distance(currentPos, startPos);

        if (!isDragging && distance > dragThreshold)
        {
            isDragging = true;
        }

    }

    public void OnBeginDrag()
    {
        Debug.Log("Entro");
        _clickPosition = sceneManager._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        var interactionHit = Physics2D.OverlapCircle(_clickPosition, 1f, _draggableItems);
        if (interactionHit == null) return;
        if(interactionHit.gameObject.TryGetComponent(out SpawningObject sP))
        {
            currentDraggable = sP;
            currentDraggable.OnBeginDrag();
        }
      
    }
    private void OnDrag(InputAction.CallbackContext contex)
    {
       

        if (currentDraggable != null)
        {
            
            _clickPosition = sceneManager._sceneCamera.ScreenToWorldPoint(contex.ReadValue<Vector2>());
            currentDraggable.OnDrag(contex.ReadValue<Vector2>());
            // currentDraggable.transform.position = Vector2.Lerp(currentDraggable.transform.position,_clickPosition,20*Time.unscaledDeltaTime);
        }
    }
    private void OnDrag(InputAction inputAction)
    {

    }
    public void OnBeginDraw()
    {
        pointerDown = true;
      //  sceneManager._dTest.BeginDraw(sceneManager._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position));
    }
    public void Drawing(InputAction.CallbackContext contex)
    {

        if (isDragging)
        {
            sceneManager._dTest.OnDraw(contex.ReadValue<Vector2>());
        }
        
    }
    public void OnEndDraw()
    {
        pointerDown = false;
        isDragging = false;
        sceneManager._dTest.OnEndDrag();
    }
    public void OnEndDrag()
    {
        if(currentDraggable != null)
        {
            currentDraggable.OnEndDrag();
            currentDraggable = null;
        }

    }
    public void OnMove()
    {
        Debug.LogError(sceneManager.playerInput.currentActionMap);
        sceneManager.OnClick(Input.GetTouch(0).position);
    }
}
