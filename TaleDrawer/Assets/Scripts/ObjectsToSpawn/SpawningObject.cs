using UnityEngine;
using UnityEngine.EventSystems;

public class SpawningObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Camera sceneCamera;
    Vector2 lastPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneCamera = Character.instance.sceneCamera;
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
        gameObject.transform.position = new Vector3(sceneCamera.ScreenToWorldPoint(eventData.position).x, sceneCamera.ScreenToWorldPoint(eventData.position).y, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    
}
