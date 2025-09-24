using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public SceneStates currentState;
    [SerializeField] DrawingTest _dTest;
    [SerializeField] GameObject _drawingDraggingCanvas;
    [SerializeField] GameObject _drawingBackground;
    public Camera _sceneCamera;
    float _blendBetweenCameras;
    [SerializeField]private CinemachineCamera _drawingCamera;
    private CinemachineFollow _drawingCFollowC;
    [SerializeField]private CinemachineCamera _playerCamera;
    private CinemachineFollow _playerCFollowC;
    [SerializeField] Camera _dCamera;
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
        _drawingCFollowC = _drawingCamera.GetComponent<CinemachineFollow>();
        _blendBetweenCameras = _sceneCamera.GetComponent<CinemachineBrain>().DefaultBlend.Time;
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
            _drawingCFollowC.enabled = true;
            //_drawingCamera.gameObject.SetActive(false);
            //_dCamera.gameObject.SetActive(false);
            //_playerCamera.gameObject.SetActive(true);            
            _dTest.gameObject.SetActive(true);
            
            Time.timeScale = 1.0f;
        }
        else if(currentState == SceneStates.Drawing)
        {
            StartCoroutine(DelayedCanvas());
            //_playerCamera.gameObject.SetActive(false);
            //_drawingCFollowC.enabled = false;
            //_drawingCamera.gameObject.SetActive(true);
            
            _drawingBackground.SetActive(true);
            
            Time .timeScale = _timeSlowDown;
        }
        else if (currentState == SceneStates.Dragging)
        {
            ExitingDrawingState();
        } 
    }

    void ExitingDrawingState()
    {
        //_drawingDraggingCanvas.SetActive(false);
        _dTest.detectTouch = false;
        _dTest.gameObject.SetActive(false);
    }
    IEnumerator DelayedCanvas()
    {        
        //yield return new WaitForSecondsRealtime(_blendBetweenCameras);
        yield return new WaitForSecondsRealtime(0f);
        _drawingDraggingCanvas.SetActive(true);
        _dTest.detectTouch = true;
    }


}

public enum SceneStates
{
    Game,
    Drawing,
    Dragging
}
