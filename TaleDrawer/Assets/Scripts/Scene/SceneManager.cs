using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public SceneStates currentState;
    [SerializeField] DrawingTest _dTest;
    [SerializeField] GameObject _drawingDraggingCanvas;
    [SerializeField] GameObject _drawingBackground;
    public Camera _sceneCamera;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _timeSlowDown;
    

    public static SceneManager instance;


    private void Awake()
    {
        instance = this;
        StateChanger(SceneStates.Game);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StateChanger(SceneStates state)
    {
        if (state == currentState) return;
        
        currentState = state;

        if(currentState == SceneStates.Game)
        {
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
            _drawingDraggingCanvas.SetActive(false);
            _dTest.gameObject.SetActive(false);
        } 
    }


}

public enum SceneStates
{
    Game,
    Drawing,
    Dragging
}
