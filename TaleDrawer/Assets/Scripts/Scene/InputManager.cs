using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
public class InputManager : MonoBehaviour
{
    [SerializeField] GameManager sceneManager;
    public SpawningObject currentDraggable;
    [SerializeField] LayerMask _draggableItems;
    Vector2 _clickPosition;
    [SerializeField] InputActionReference MoveMouse;
    [SerializeField] InputActionReference Draw;
    [SerializeField] InputActionReference Move;
    [Header("Settings")]
    public float dragThreshold = 20f;

    private Vector2 startPos;
    private Vector2 startWorldPos;
    public bool isDragging = false;
    public Rubber rubber;
    private bool pointerDown = false;
    public EventSystem eventSystem;
    public GraphicRaycaster raycaster;
    public Character character;
    public Canvas canvas;

    public PlayerInput playerInput;
    public Vector2 input;
    public Vector2 mouseInput;
    public Action onGoingDown;
    public TemplateBook templateBook;

    public static InputManager instance;

    public bool isAiming;
    public Vector2 startAimPos;
    public float aimForce;
    public Vector2 aimForceVector;
    public float aimAngle;
    float currentInput;
    public RectTransform cursorImage;
    Vector2 position = Vector2.zero;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Cursor.visible = false;
        //MoveMouse.action.performed += OnDrag;
        //Draw.action.performed += Drawing;
        //Move.action.performed += OnMove;
    }

    // Update is called once per frame

    public void OnInput()
    {

    }
    private void Update()
    {

        character.xInput = playerInput.actions["Move"].ReadValue<Vector2>().x;
        

        Debug.LogError(character.xInput);
        character.climbingInputs = playerInput.actions["Climb"].ReadValue<Vector2>();
        mouseInput = playerInput.actions["Input"].ReadValue<Vector2>();
        character.glidingInputs = playerInput.actions["Glide"].ReadValue<Vector2>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorImage.parent.GetComponent<RectTransform>(), InputManager.instance.mouseInput, null, out position);
        cursorImage.anchoredPosition = position;
        if (!pointerDown) return;

        Vector2 currentPos = sceneManager._sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(currentPos, startPos);

        if (!isDragging && distance > dragThreshold)
        {
            Debug.LogError(distance);
            isDragging = true;
        }

        if (isAiming)
        {
            
            aimForce = Mathf.Min(10, Vector2.Distance(startAimPos, mouseInput));
            aimAngle = Vector2.Angle(startAimPos - mouseInput, Vector2.right);
            aimAngle *= Mathf.Deg2Rad;        
            aimForceVector = (startAimPos - mouseInput).normalized * Mathf.Clamp(aimForce, 0, 25) * 2;
            character.TrayectoryVisuals(aimForceVector);
        }

    }

    

    public void ContinueDialogue()
    {

    }

    public void OnOpenBook()
    {
        if (!templateBook.gameObject.activeSelf)
        {
            templateBook.gameObject.SetActive(true);
            templateBook.OnActivated();
        }
        else
        {
            templateBook.OnDeactivated();
            templateBook.gameObject.SetActive(false);
        }

    }
    public void OnInteract()
    {
        if (sceneManager.currentState == SceneStates.Dialogue)
        {
            DialogManager.instance.Continue();
        }
        else
        {
            if (character.currentInteractable != null)
            {
                character.currentInteractable.Interact();
                Debug.LogError("INTERACT");
            }
            else
            {
                Debug.LogError("No tengo II");

            }
        }

            
    }
    public void OnJump()
    {
        Debug.LogError("LAPUTAMADRE");
        
        character.characterModel.Jump();
       // character.SendInputToFSM(CharacterStates.Jumping);
    }

    public void OnGoingDown()
    {
        onGoingDown?.Invoke();
    }

    public void OnBeginDrag()
    {
        
        _clickPosition = sceneManager._sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        var interactionHit = Physics2D.OverlapCircle(_clickPosition, 1f, _draggableItems);
        if (interactionHit == null) return;
        if(interactionHit.gameObject.TryGetComponent(out SpawningObject sP))
        {
           // Debug.LogError("LAPUTAMADRE");
            currentDraggable = sP;
            currentDraggable.OnBeginDrag();
            GameManager.instance.SetCursorSprite(CursorTypes.ClosedHand);
        }
      
    }
    public void OnDrag()
    {
       


        if (currentDraggable != null)
        {

            _clickPosition = sceneManager._sceneCamera.ScreenToWorldPoint(mouseInput);
            currentDraggable.OnDrag(mouseInput);
            // currentDraggable.transform.position = Vector2.Lerp(currentDraggable.transform.position,_clickPosition,20*Time.unscaledDeltaTime);
        }
    }

    public void OnAim()
    {

        if (CustomTools.IsTouchOverUI(mouseInput))
        {

            return;
        }
        if (character._currentState == CharacterStates.Boleadoras)
        {
            isAiming = true;
            startAimPos = mouseInput;
        }
    }

    public void OnShoot()
    {
        if (isAiming)
        {
            character.Shoot(aimForceVector);
            isAiming = false;
        }
    }

    public void OnBeginDraw()
    {
        pointerDown = true;
        startPos = sceneManager._sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        startWorldPos = Input.mousePosition;


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


    }
    public void OnDraw()
    {

        if (isDragging)
        {
            if (rubber != null)
            {

                rubber.OnDrag(mouseInput);

            }
            else
            {
                if (character._currentState == CharacterStates.Boleadoras) return;
                sceneManager._dTest.OnDraw(mouseInput);
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
    public void OnTest()
    {
        Debug.LogError("LAPUTAMADRE");
    }
    public void OnEndDrag()
    {
        if(currentDraggable != null)
        {
            GameManager.instance.SetCursorSprite(CursorTypes.Brush);
            currentDraggable.OnEndDrag();
            currentDraggable = null;
           
        }

    }
    public void OnMove()
    {

    }

    public void OnClimb() 
    {
            
    }

    public void OnBeginClimb()
    {
        if(Character.instance.TryGetComponent(out Robin rob) && rob.canClimb && Character.instance._climbingTransitions.HasFlag(Character.instance._currentState) && Character.instance.grounded)
        {            
            Character.instance.currentInteractable.Interact();
        }
    }

    public void OnEndClimb()
    {
        if(Character.instance.TryGetComponent(out Robin rob) && rob.canClimb && Character.instance._currentState == CharacterStates.OnLadder && Character.instance.grounded)
        {
            rob.characterView.OnIdle();
            Character.instance.SendInputToFSM(CharacterStates.Moving);
        }
    }

    public void OnGlide()
    {

    }

    public void OnEndGlide()
    {
        if(Character.instance.TryGetComponent(out Robin rob) && rob._currentState == CharacterStates.Glide)
        {
            rob.SendInputToFSM(CharacterStates.Idle);
        }
    }
}
