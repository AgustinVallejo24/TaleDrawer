using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class SceneManager : MonoBehaviour
{
    public SceneStates currentState;
    [SerializeField] DrawingTest _dTest;
    [SerializeField] GameObject _drawingDraggingCanvas;
    [SerializeField] GameObject _drawingBackground;
    public Camera _sceneCamera;
    float _blendBetweenCameras;    
    [SerializeField]private CinemachineCamera _playerCamera;
    private CinemachineFollow _playerCFollowC;
    
    [SerializeField] float _playerCameraOriginalSize;
    [SerializeField] float _playerCameraDrawingSize;
    [SerializeField] float _cameraResizingTime;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _timeSlowDown;
    public Character levelCharacter;
    

    public static SceneManager instance;

    Vector2 _clickPosition;
    [SerializeField] float _clickRayLength;
    [SerializeField] LayerMask _piso;
    [SerializeField] LayerMask _interactables;
    [SerializeField]public List<CustomNode> nodes;

    bool gameTouch;

    public GameObject sticker;

    public Vector2 touchPosition;

    public float drawingThreshold;
    private void Awake()
    {
        instance = this;
        StateChanger(SceneStates.Game);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerCFollowC = _playerCamera.GetComponent<CinemachineFollow>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.touchCount > 0 && (Character.instance._currentState == CharacterStates.Idle || Character.instance._currentState == CharacterStates.Moving || Character.instance._currentState == CharacterStates.Wait))
        {
            UnityEngine.Touch touch = Input.GetTouch(0);
           
            if (currentState == SceneStates.Drawing)
            {
                _dTest.Draw(Input.touchCount, touch);
            }
            switch (touch.phase)
            {
                case TouchPhase.Began:

                   if(currentState == SceneStates.Game)
                    {
                        touchPosition = touch.position;
                        gameTouch = true;
                    }
                    else
                    {
                        gameTouch = false;
                    }
                    break;

                case TouchPhase.Moved:
                    if(currentState == SceneStates.Game && Vector2.Distance(touchPosition, touch.position)> drawingThreshold)
                    {
                        StateChanger(SceneStates.Drawing);
                        _dTest.gameObject.SetActive(true);
                    }
                    break;
                case TouchPhase.Stationary:

                    break;

                case TouchPhase.Ended:
                   
                    if (currentState == SceneStates.Game && gameTouch)
                    {
                        _clickPosition = _sceneCamera.ScreenToWorldPoint(touch.position);
                        Ray interactionRay = _sceneCamera.ScreenPointToRay(touchPosition);
                        var interactionHit = Physics2D.OverlapCircle(_clickPosition, 1f,_interactables);
                     
                        if(interactionHit != null && interactionHit.gameObject.TryGetComponent(out IInteractable interactable))
                        {
                            interactable.InteractWithPlayer();
                        }
                        else
                        {
                            RaycastHit2D hit = Physics2D.Raycast(_clickPosition, Vector2.down, _clickRayLength, _piso);

                            if (hit)
                            {

                                CustomNode goal = CustomTools.GetClosestNode(hit.point, nodes);
                                if (Character.instance.GetPath(goal, new Vector2(hit.point.x, hit.transform.GetComponent<Collider2D>().bounds.max.y + 1f)))
                                {
                                    Character.instance.SendInputToFSM(CharacterStates.Moving);
                                    sticker.transform.position = new Vector2(hit.point.x, hit.transform.GetComponent<Collider2D>().bounds.max.y + .5f);
                                    sticker.SetActive(true);
                                }
                                else
                                {
                                    sticker.SetActive(false);

                                }

                            }
                            else
                            {
                                sticker.SetActive(false);
                            }
                        }

                       
                    }

                    break;

                case TouchPhase.Canceled:
                    break;

            }


            
        }

    }

    public void StateChanger(SceneStates state)
    {
        SceneStates previousState = currentState;
        if (state == currentState) return;
        
        currentState = state;

        if(currentState == SceneStates.Game)
        {
            if(previousState == SceneStates.Drawing)
            {
                ExitingDrawingState();
            }            
            _drawingBackground.SetActive(false);            
            
                      
          //  _dTest.gameObject.SetActive(true);
            
            Time.timeScale = 1.0f;
        }
        else if(currentState == SceneStates.Drawing)
        {
            
            _drawingBackground.SetActive(true);
            _drawingDraggingCanvas.SetActive(true);

            Time .timeScale = _timeSlowDown;
        }
        else if (currentState == SceneStates.Dragging)
        {
            ExitingDrawingState();
        } 
    }

    void ExitingDrawingState()
    {
        
        _drawingDraggingCanvas.SetActive(false);        
        _dTest.gameObject.SetActive(false);

        
    }

    
    /*void EnteringDrawingState()
    {
       
        
        
    }*/

    

}

public enum SceneStates
{
    Game,
    Drawing,
    Dragging
}
