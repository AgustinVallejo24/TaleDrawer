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
    [SerializeField] SpriteRenderer _mySpriteRenderer;
    [SerializeField] LayerMask _interactuables;
    public IInteractable _currentInteractuable;
    public string _intrectableName;
    Color _originalColor;
    bool _first = true;
    bool _second = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneCamera = SceneManager.instance._sceneCamera;
        _myColl = GetComponent<Collider2D>();        
        _myColl.isTrigger = true;
        _originalColor = _mySpriteRenderer.color;

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

            var hit = Physics2D.OverlapCircle(transform.position, transform.localScale.y, _interactuables);
            if (hit && hit.TryGetComponent<IInteractable>(out IInteractable interctuable))
            {
                if (_first)
                {
                    _first = false;
                    _second = true;
                    _currentInteractuable = interctuable;
                    _intrectableName = _currentInteractuable.GetType().Name;
                    _mySpriteRenderer.color = Color.green;
                }
                
            }
            else
            {
                if (_second)
                {
                    _second = false;
                    _first = true;
                    _currentInteractuable = null;
                    _intrectableName = "None";
                    _mySpriteRenderer.color = _originalColor;
                }
                
            }
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SceneManager.instance.StateChanger(SceneStates.Game);
        _mySpriteRenderer.color = _originalColor;
        _myColl.isTrigger = objectIsTrigger;
        _mySpriteRenderer.sortingOrder = 1;
        if (objectUseGravity && _myrb != null)
        {
            _myrb.gravityScale = 1;
        }

        if(_currentInteractuable != null)
        {
            _currentInteractuable.Interact(this);
        }
        
    }

    
}
