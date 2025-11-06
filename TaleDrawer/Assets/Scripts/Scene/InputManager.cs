using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    [SerializeField] SceneManager sceneManager;
    public SpawningObject currentDraggable;
    [SerializeField] LayerMask _draggableItems;
    Vector2 _clickPosition;
    [SerializeField] InputActionReference MoveMouse;
    void Start()
    {
        MoveMouse.action.performed += OnDrag;
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void OnEndDrag()
    {
        if(currentDraggable != null)
        {
            currentDraggable.OnEndDrag();
            currentDraggable = null;
        }

    }
}
