using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class SpawningObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Camera sceneCamera;    
    public SpawnableObjectType myType;
    public Collider2D _myColl;
    public bool objectIsTrigger;
    public Rigidbody2D _myrb;
    public bool objectUseGravity;
    [SerializeField] SpriteRenderer _mySpriteRenderer;
    [SerializeField] LayerMask _interactuables;
    public IInteractable _currentInteractuable;
    public string _intrectableName;
    Color _originalColor;
    bool _first = true;
    bool _second = true;
    public float weight;
    public Action<float, GameObject> interactableDelegate;
    GridPoint _currentGridP;
    GridPoint _previousGridP;
    Vector2 _pointPos;
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
        var tuple = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position);
        _currentGridP = tuple.Item3;
        if (_currentGridP == null)
        {
            _currentGridP = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position, Mathf.Infinity).Item3;
        }
        else
        {
            _currentGridP.SelectAndDeselectPoint(tuple.Item1);
            _pointPos = _currentGridP.transform.position;
        }
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
                    if(_currentGridP != null)
                    {
                        _currentGridP.SelectAndDeselectPoint(false);
                    }

                    if(_previousGridP != null)
                    {
                        _previousGridP.SelectAndDeselectPoint(false);
                    }
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

                /*if(_currentGridP == null)
                {
                    var tuple = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position, Mathf.Infinity);
                    _currentGridP = tuple.Item3;
                    if (_currentGridP != null)
                    {
                        _currentGridP.SelectAndDeselectPoint(tuple.Item1);
                        _pointPos = _currentGridP.transform.position;
                    }

                    if(_previousGridP != null)
                    {
                        _previousGridP.SelectAndDeselectPoint(false);
                    }
                }*/
                if (Vector2.Distance(_currentGridP.transform.position, transform.position) > 0.7)
                {
                    var tuple = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position);

                    if (tuple.Item1)
                    {
                        if (_currentGridP != null)
                            _previousGridP = _currentGridP;

                        _currentGridP = tuple.Item3;


                        if (_currentGridP != _previousGridP)
                        {
                            _previousGridP.SelectAndDeselectPoint(false);

                            if (_currentGridP != null)
                            {
                                _currentGridP.SelectAndDeselectPoint(tuple.Item1);
                                _pointPos = _currentGridP.transform.position;
                            }

                        }
                    }
                    else
                    {                        
                        _previousGridP?.SelectAndDeselectPoint(false);
                        _currentGridP?.SelectAndDeselectPoint(false);
                    }
                    

                }              
                
            }
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var tuple = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position);
        _currentGridP = tuple.Item3;      

        if(_currentInteractuable != null)
        {
            _currentInteractuable.Interact(this);
            _currentGridP = null;
        }
        else if(_currentGridP != null && tuple.Item1)
        {
            _currentGridP.SelectAndDeselectPoint(false);
            transform.position = new Vector3(_currentGridP.transform.position.x, _currentGridP.transform.position.y, 0);
            /*_currentGridP = null;
            _previousGridP = null;*/
        }

        if(_currentInteractuable != null || (_currentGridP != null && tuple.Item1))
        {
            SceneManager.instance.StateChanger(SceneStates.Game);
            _mySpriteRenderer.color = _originalColor;
            _myColl.isTrigger = objectIsTrigger;
            _mySpriteRenderer.sortingOrder = 1;
            if (objectUseGravity && _myrb != null)
            {
                _myrb.gravityScale = 1;
            }
        }
        
    }

    
}
