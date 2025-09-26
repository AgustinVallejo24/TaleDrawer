using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

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
        
    }

    public void StateChanger(SceneStates state)
    {
        SceneStates previousState = currentState;
        if (state == currentState) return;
        
        currentState = state;

        if(currentState == SceneStates.Game)
        {
            if(previousState != SceneStates.Dragging)
            {
                ExitingDrawingState();
            }            
            _drawingBackground.SetActive(false);            
            
                      
            _dTest.gameObject.SetActive(true);
            
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

    
    void EnteringDrawingState()
    {
       
        
        
    }


}

public enum SceneStates
{
    Game,
    Drawing,
    Dragging
}
