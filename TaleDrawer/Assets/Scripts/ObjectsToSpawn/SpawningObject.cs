using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawningObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Camera sceneCamera;    
    public SpawnableObjectType myType;
    Collider2D _myColl;
    public bool objectIsTrigger;
    Rigidbody2D _myrb;
    public bool objectUseGravity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneCamera = SceneManager.instance._sceneCamera;
        _myColl = GetComponent<Collider2D>();
        _myColl.isTrigger = true;

        if(TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            _myrb = rb;

            _myrb.gravityScale = 0;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {  
        if(SceneManager.instance.currentState == SceneStates.Dragging)
        {
            
            gameObject.transform.position = new Vector3(sceneCamera.ScreenToWorldPoint(eventData.position).x, sceneCamera.ScreenToWorldPoint(eventData.position).y, 0f);
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SceneManager.instance.StateChanger(SceneStates.Game);
        _myColl.isTrigger = objectIsTrigger;
        if (objectUseGravity && _myrb != null)
        {
            _myrb.gravityScale = 1;
        }
        
    }

    
}
