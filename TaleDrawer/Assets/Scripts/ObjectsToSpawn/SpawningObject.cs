using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
public class SpawningObject : MonoBehaviour, IDeletable
{
    public Camera sceneCamera;    
    public SpawnableObjectType myType;
    public Collider2D _myColl;
    public bool objectIsTrigger;
    public Rigidbody2D _myrb;
    public bool objectUseGravity;
    [SerializeField] SpriteRenderer _mySpriteRenderer;
    [SerializeField] LayerMask _interactuables;
    [SerializeField] LayerMask _obstacleMask;
    public IInteractable _currentInteractuable;
    public string _intrectableName;
    Color _originalColor;
    bool _first = true;
    bool _second = true;
    bool _third = true;
    [SerializeField] bool _canIntercactWithEntity;
    [SerializeField] bool _interactingWithEntity;
    [SerializeField] bool _canSpawn;
    public float weight;
    public Action<float, GameObject> interactableDelegate;
    GridPoint _currentGridP;
    GridPoint _previousGridP;
    Vector2 _pointPos;
    protected Character _myCharacter;
    [SerializeField]protected bool _spawned = false;
    [SerializeField] protected LayerMask _objectMask;

    [SerializeField] public Animator myAnim;

    [SerializeField] protected Entity _currentEntity;

    public ExplosionParticle clouds;

    public virtual void Awake()
    {
        _originalColor = _mySpriteRenderer.color;
    }

    public virtual void Start()
    {
        _myCharacter = Character.instance;
        sceneCamera = GameManager.instance._sceneCamera;
        _myColl = GetComponent<Collider2D>();        
        _myColl.isTrigger = true;
        

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

    public void SetTransparency(Color alpha, int value = 0)
    {
        if(value == 0)
        {
            Color newColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, alpha.a);
            _mySpriteRenderer.color = newColor;
        }
        else
        {
            _mySpriteRenderer.color = _originalColor;
        }
        
    }

    public virtual void Delete()
    {
        GameManager.instance.RemoveSpawningObjectFromList(this);
        if(clouds != null)
        {
            Instantiate(clouds, transform.position, Quaternion.identity);
        }
        

    }

    public virtual void Paint()
    {
       GameManager.instance.StateChanger(SceneStates.Dragging);
    }

    public void OnBeginDrag()
    {
        if(GameManager.instance.currentState == SceneStates.Dragging && !_spawned)
        {       
            _mySpriteRenderer.color = _originalColor;
            if (_myColl != null)
            {                
                _myColl.enabled = false;
            }            

            Debug.Log("Arrastre iniciado y Collider deshabilitado.");
        }
        
    }

    public void OnDrag(Vector2 position)
    {  

        if(GameManager.instance.currentState == SceneStates.Dragging && !_spawned)
        {            
           transform.position = Vector2.Lerp(transform.position, sceneCamera.ScreenToWorldPoint(position), .5f);

            var hit = Physics2D.OverlapCircle(transform.position, transform.localScale.y/2, _interactuables);
            //var hit2 = Physics2D.OverlapCircle(transform.position, transform.localScale.y/2, _objectMask);

            if (Physics2D.OverlapCircle(transform.position, transform.localScale.y / 2, _obstacleMask))
            {
                _first = true;
                _second = true;
                _third = true;
                _canSpawn = false;
                _mySpriteRenderer.color = Color.gray;
            }
            else if (hit && hit.TryGetComponent<IInteractable>(out IInteractable interctuable))
            {
                if (_first)
                {
                    _first = false;
                    _second = true;
                    _third = true;
                    _canSpawn = true;
                    _currentInteractuable = interctuable;
                    _intrectableName = _currentInteractuable.GetType().Name;
                    _mySpriteRenderer.color = Color.green;
                    _interactingWithEntity = false;
                    _currentEntity = null;
                }
                
            }
            else if(_canIntercactWithEntity && hit && hit.TryGetComponent<Entity>(out Entity ent))
            {
                if (_third)
                {
                    _third = false;
                    _first = true;
                    _second = true;
                    _canSpawn = true;
                    _interactingWithEntity = true;
                    _currentInteractuable = null;
                    _intrectableName = "None";
                    _currentEntity = ent;
                    _mySpriteRenderer.color = Color.green;
                    
                }
            }
            else
            {
                if (_second)
                {
                    _second = false;
                    _first = true;
                    _third = true;
                    _canSpawn = true;                    
                    _currentInteractuable = null;
                    _currentEntity = null;
                    _intrectableName = "None";
                    _interactingWithEntity = false;                    
                }

                _mySpriteRenderer.color = _originalColor;

                /*if(hit && ((1 << hit.gameObject.layer) & _obstacleMask) != 0)
                {
                    _canSpawn = false;
                    _mySpriteRenderer.color = Color.gray;
                }
                else
                {
                    _canSpawn = true;
                    _mySpriteRenderer.color = _originalColor; 
                }*/

            }
        }
        
    }

    public void OnEndDrag()
    {
        if(GameManager.instance.currentState == SceneStates.Dragging && !_spawned)
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
            

            

            if (_currentInteractuable != null || _interactingWithEntity || _canSpawn)
            {
                
                _spawned = true;
                GameManager.instance.StateChanger(SceneStates.Game);
                Instantiate(clouds,transform.position,Quaternion.identity);
                _mySpriteRenderer.material.SetFloat("_NoiseValue", 0);
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

                GameManager.instance.AddSpawningObject(this);

                if (_interactingWithEntity)
                {
                    InteractionWithEntity();
                    _currentInteractuable = null;
                    
                    
                }
                else if (_currentInteractuable != null)
                {
                    _currentInteractuable.Interact(this);
                    
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
       // _mySpriteRenderer.color = Color.red;
        _currentInteractuable = null;
        _intrectableName = "";

        var tuple = PlacementGridManager.Instance.FindNearestValidPlacement(transform.position, Mathf.Infinity, false);
        transform.position = new Vector3(tuple.Item3.transform.position.x, tuple.Item3.transform.position.y, 0);
    }

    public virtual void InteractionWithEntity()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (_spawned)
        {
            if (collision.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(myType, gameObject);
            }
            else if (collision.gameObject.TryGetComponent(out SpawningObject spawningObject) && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y + .5f, _objectMask) && spawningObject.weight > 1f)
            {
                Destroy(spawningObject.gameObject);
            }
            
        }


    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (_spawned)
        {
            if (collision.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(myType, gameObject);
            }
            
            /*if (collision.gameObject.TryGetComponent(out SpawningObject spawningObject) && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y + .5f, _objectMask) && spawningObject.weight > 1f)
            {
                Destroy(spawningObject.gameObject);
            }*/
        }
    }


    public void SyncColiiders()
    {

    }
}
