using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public SceneStates currentState;
    [SerializeField] CharacterStates _clicableStates;
    public DrawingTest _dTest;
    [SerializeField] GameObject _drawingDraggingCanvas;
    [SerializeField] GameObject _drawingBackground;
    [SerializeField] GameObject _blurEffect;
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
    public PlayerInput playerInput;

    public static GameManager instance;

    Vector2 _clickPosition;
    [SerializeField] float _clickRayLength;
    [SerializeField] LayerMask _piso;
    [SerializeField] LayerMask _clickable;
    [SerializeField] LayerMask _interactables;
    [SerializeField]public List<CustomNode> nodes;

    bool gameTouch;

    public GameObject sticker;

    public Vector2 touchPosition;

    public float drawingThreshold;

    public Dialogue levelDialogue;


    [SerializeField] SpawnableManager _spawnableManager;
    [SerializeField] ZernikeManager _zernikeManager;

    [SerializeField] List<SpawningObject> _spawnedObjects;
    [SerializeField] int _maxSpawnedObjects;
    [SerializeField] Color _transparencyColor;

    [SerializeField] InitialCameraAnimation cameraAnimation;
    public bool playInitialAnimation;

    [SerializeField] Image _blackImage;
    private void Awake()
    {
        instance = this;
        StateChanger(SceneStates.Game);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Desactiva VSync para permitir el control manual
        QualitySettings.vSyncCount = 0;
        // Establece el límite a 60 FPS
        Application.targetFrameRate = 60;
        _playerCFollowC = _playerCamera.GetComponent<CinemachineFollow>();
     //   _clickRayLength = levelCharacter.transform.localScale.y * 2;
        _zernikeManager.recognitionAction = _dTest.StartDissolve;


        if (playInitialAnimation)
        {
            levelCharacter.SendInputToFSM(CharacterStates.Stop);
            cameraAnimation.CameraSequence();
        }
        else
        {

            _playerCamera.enabled = true;
        }
    }

    public void AddSpawningObject(SpawningObject obj)
    {
        if(_spawnedObjects.Count >= _maxSpawnedObjects)
        {
            SpawningObject sp = _spawnedObjects.First();
            _spawnedObjects.Remove(_spawnedObjects.First());
            sp.Delete();
            _spawnedObjects.First().SetTransparency(_transparencyColor);
        }
        else if(_spawnedObjects.Count == _maxSpawnedObjects - 1)
        {
            _spawnedObjects.First().SetTransparency(_transparencyColor);
        }

        _spawnedObjects.Add(obj);
    }
    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }
    public IEnumerator FadeOutCoroutine()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            _blackImage.color = new Color(0, 0, 0, t / 1);
            yield return null;
        }
    }
    public void RemoveSpawningObjectFromList(SpawningObject obj)
    {
        if (_spawnedObjects.Contains(obj))
        {
            _spawnedObjects.Remove(obj);
        }
        
    }


    // Update is called once per frame
   

    public void OnClick(Vector2 position)
    {
        if (currentState == SceneStates.Game && _clicableStates.HasFlag(Character.instance._currentState) && !_dTest.isDrawing)
        {
            if(levelCharacter.currentInteractable != null)
            {
                levelCharacter.currentInteractable.InsideInteraction();
                return;
            }
            _clickPosition = _sceneCamera.ScreenToWorldPoint(position);
            var interactionHit = Physics2D.OverlapCircle(_clickPosition, 1f, _interactables);

            if (interactionHit != null && interactionHit.gameObject.TryGetComponent(out IInteractable interactable))
            {
        
                interactable.InteractWithPlayer();
            }
            else
            {
                var hit2 = Physics2D.OverlapCircle(_clickPosition, .2f, _clickable);

                if (!hit2) return;

                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(_piso);   // Tu layer mask
                filter.useTriggers = false;

                RaycastHit2D[] results = new RaycastHit2D[1];

                int hits = Physics2D.Raycast(_clickPosition, Vector2.down, filter, results, _clickRayLength);
                

    
                if (hits>0)
                {
                    Debug.LogError("LPM");
                    RaycastHit2D hit = results[0];
                    CustomNode goal = CustomTools.GetClosestNode(hit.point, nodes.Where(x => x.isClickable == true).ToList());


                }
                else
                {
                    sticker.SetActive(false);
                }
            }


        }
    }

    public void SpawnObject(SpawnableObjectType objectType)
    {
        StateChanger(SceneStates.Spawning);
        playerInput.SwitchCurrentActionMap("Drawing");
        GameObject newObj = Instantiate(_spawnableManager.spawnableObjectDictionary[objectType].gameObject, _dTest.currentCentroid, Quaternion.identity);



          newObj.GetComponent<SpawningObject>().Paint();

         
      
    }
    public void ChangeToDrawingState()
    {
        if (currentState == SceneStates.Drawing) return;
        currentState = SceneStates.Drawing;
        playerInput.SwitchCurrentActionMap("Drawing");
        _drawingBackground.SetActive(true);
        _drawingDraggingCanvas.SetActive(true);

        Time.timeScale = _timeSlowDown;

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
            playerInput.SwitchCurrentActionMap("Movement");
       //    PlacementGridManager.Instance.SetGridVisibility(false);
                        
            
                      
          //  _dTest.gameObject.SetActive(true);
            
            Time.timeScale = 1.0f;
        }
        else if(currentState == SceneStates.Drawing)
        {

            //  _drawingBackground.SetActive(true);
            _blurEffect.SetActive(true);
           // _drawingDraggingCanvas.SetActive(true);
          //  playerInput.SwitchCurrentActionMap("Drawing");
            Time .timeScale = _timeSlowDown;
        }
        else if (currentState == SceneStates.Dragging)
        {
            playerInput.SwitchCurrentActionMap("Dragging");
            //PlacementGridManager.Instance.RefreshGridAvailability();
          //  PlacementGridManager.Instance.SetGridVisibility(true);
            ExitingDrawingState();
        } 
    }

    void ExitingDrawingState()
    {
        _blurEffect.SetActive(false);
        _dTest.ResetLineRenderers();
        //_drawingDraggingCanvas.SetActive(false);        
        //_drawingBackground.SetActive(false);


    }

    
    /*void EnteringDrawingState()
    {
       
        
        
    }*/

    

}

public enum SceneStates
{
    Game,
    Drawing,
    Dragging,
    Event,
    GameOver,
    Spawning,
    Dialogue,
}
