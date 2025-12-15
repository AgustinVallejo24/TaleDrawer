using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
public class InputManager : MonoBehaviour
{
    [SerializeField] GameManager sceneManager;
    public SpawningObject currentDraggable;
    [SerializeField] LayerMask _draggableItems;
    Vector2 _clickPosition;
    [SerializeField] InputActionReference MoveMouse;
    [SerializeField] InputActionReference Draw;

    [Header("Settings")]
    public float dragThreshold = 20f;

    private Vector2 startPos;
    private Vector2 startWorldPos;
    public bool isDragging = false;
    public Rubber rubber;
    private bool pointerDown = false;
    public EventSystem eventSystem;
    public GraphicRaycaster raycaster;

    public Canvas canvas;
    void Start()
    {
        MoveMouse.action.performed += OnDrag;
        Draw.action.performed += Drawing;
    }

    // Update is called once per frame


    private void Update()
    {
        if (!pointerDown) return;

        Vector2 currentPos = sceneManager._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        float distance = Vector2.Distance(currentPos, startPos);

        if (!isDragging && distance > dragThreshold)
        {
            Debug.LogError(distance);
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

    public void OnBeginDraw()
    {
        pointerDown = true;
        startPos = sceneManager._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        startWorldPos = Input.GetTouch(0).position;


        PointerEventData data = new PointerEventData(eventSystem);
        data.position = startWorldPos;

        List<RaycastResult> resultados = new List<RaycastResult>();
        raycaster.Raycast(data, resultados);

        foreach (var r in resultados)
        {
            if (r.gameObject.TryGetComponent(out Rubber rub) && !rub.isMoving)
            {
                rubber = rub;
            }
        }

        //  sceneManager._dTest.BeginDraw(sceneManager._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position));
    }
    public void Drawing(InputAction.CallbackContext contex)
    {

        if (isDragging)
        {
            if (rubber != null)
            {
 
                rubber.OnDrag(contex);

            }
            else
            {
                sceneManager._dTest.OnDraw(contex.ReadValue<Vector2>());
            }
 



        

            
        }
        
    }
    public void OnEndDraw()
    {
        pointerDown = false;
        isDragging = false;
        if(rubber != null)
        {
            rubber.OnRelease();
            rubber = null;
        }
        
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
        if(rubber==null)
        sceneManager.OnClick(Input.GetTouch(0).position);
    }
}
