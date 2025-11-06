using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class SpawningObject : MonoBehaviour
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
    protected Character _myCharacter;
    [SerializeField]protected bool _spawned = false;
    [SerializeField] protected LayerMask _objectMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _myCharacter = Character.instance;
        sceneCamera = SceneManager.instance._sceneCamera;
        _myColl = GetComponent<Collider2D>();        
        _myColl.isTrigger = true;
        _originalColor = _mySpriteRenderer.color;

        if(TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            _myrb = rb;

            _myrb.gravityScale = 0;
        }

        if (_myColl == null)
        {
            Debug.LogError("¡El objeto arrastrable necesita un Collider2D!");
        }
        else
        {
            // Debugging: Confirmar estado inicial del Collider
            Debug.Log($"[Dragger Start] Collider inicializado. Estado: {_myColl.enabled}");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag()
    {
        if(SceneManager.instance.currentState == SceneStates.Dragging && !_spawned)
        {       
            _mySpriteRenderer.color = _originalColor;
            if (_myColl != null)
            {                
                _myColl.enabled = false;
            }

            var tuple = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position);
            _currentGridP = tuple.Item3;
            if (_currentGridP == null)
            {
                _currentGridP = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position, Mathf.Infinity).Item3;
            }
            else
            {
                //_currentGridP.SelectAndDeselectPoint(tuple.Item1);
                _pointPos = _currentGridP.transform.position;
            }

            Debug.Log("Arrastre iniciado y Collider deshabilitado.");
        }
        
    }

    public void OnDrag(Vector2 position)
    {  

        if(SceneManager.instance.currentState == SceneStates.Dragging && !_spawned)
        {            
           transform.position = Vector2.Lerp(transform.position, sceneCamera.ScreenToWorldPoint(position), .5f);

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

                    if (_previousGridP != null && !_previousGridP.blocked) _previousGridP.SelectAndDeselectPoint(false);
                    if (!_currentGridP.blocked) _currentGridP.SelectAndDeselectPoint(false);
                    
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
                            if(!_previousGridP.blocked)_previousGridP.SelectAndDeselectPoint(false);

                            if (_currentGridP != null)
                            {
                                _currentGridP.SelectAndDeselectPoint(tuple.Item1);
                                _pointPos = _currentGridP.transform.position;
                            }

                        }
                    }
                    else
                    {
                        if(_previousGridP != null && !_previousGridP.blocked) _previousGridP.SelectAndDeselectPoint(false);
                        if (!_currentGridP.blocked) _currentGridP.SelectAndDeselectPoint(false);
                    }
                    

                }              
                
            }
        }
        
    }

    public void OnEndDrag()
    {
        if(SceneManager.instance.currentState == SceneStates.Dragging && !_spawned)
        {
            if (_myrb != null)
            {
                _myrb.position = transform.position; // Sincroniza directamente la posición del Rigidbody
                Debug.LogWarning("[SYNC] Rigidbody position forced to sync.");
            }
            else
            {
                if (_myColl != null)
                {
                    _myColl.enabled = true; // Habilitar primero


                    Physics2D.SyncTransforms();
                    Debug.LogWarning("[SYNC] Forced Physics2D SyncTransforms after placement.");
                }
            }
            var tuple = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position);
            _currentGridP = tuple.Item3;

            if (_currentInteractuable != null)
            {
                _currentInteractuable.Interact(this);
                _currentGridP = null;
            }
            else if (_currentGridP != null && tuple.Item1)
            {
                _currentGridP.SelectAndDeselectPoint(false);
                transform.position = new Vector3(_currentGridP.transform.position.x, _currentGridP.transform.position.y, 0);
                /*_currentGridP = null;
                _previousGridP = null;*/
            }

            if (_currentInteractuable != null || (_currentGridP != null && tuple.Item1))
            {
                _spawned = true;
                SceneManager.instance.StateChanger(SceneStates.Game);
                _mySpriteRenderer.color = _originalColor;
                if (_myColl != null)
                {
                    _myColl.enabled = true;
                    _myColl.isTrigger = objectIsTrigger;
                }
                _mySpriteRenderer.sortingOrder = 1;
                if (objectUseGravity && _myrb != null)
                {
                    _myrb.gravityScale = 1;
                }
            }
            else
            {

                if (_myColl != null)
                {
                    Debug.LogError("Me quedo vegetativo");
                    _myColl.enabled = true;
                }
            }

            Debug.Log("Arrastre finalizado y Collider habilitado. Objeto colocado.");
        }
        
        
    }

    public void CantInteract()
    {
        _mySpriteRenderer.color = Color.red;
        _currentInteractuable = null;
        _intrectableName = "";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_spawned)
        {
            if (collision.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(myType, gameObject);
            }
            if (collision.gameObject.TryGetComponent(out SpawningObject spawningObject) && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y + .5f, _objectMask) && spawningObject.weight > 1f)
            {
                Destroy(spawningObject.gameObject);
            }
        }


    }

    public void SyncColiiders()
    {

    }
}
