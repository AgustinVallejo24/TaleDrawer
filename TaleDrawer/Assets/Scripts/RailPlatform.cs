using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
public class RailPlatform : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] Transform[] _originalWaypoints;
    [SerializeField] Transform[] _extraWaypoints;
    [SerializeField] Transform[] _waypoints;
    [SerializeField] NewSerializableDictionary<int, NewSerializableDictionary<int, int>> _pathIntersectionWaypoints;

    [SerializeField] float _speed = 3f;
    [SerializeField] float _maxDistance = 0.1f;
    [SerializeField] float _waitTime = 1.5f;
    [SerializeField] float _activationDelayTime = 2f;

    [Header("Comportamiento")]
    [SerializeField] bool _active = true;
    [SerializeField] bool _stopWhenPathFinished = false;
    [SerializeField] bool _needsPlayer = false;
    [SerializeField] bool _isChanging = false;

    [SerializeField] int _currentWaypoint = 0;
    private int _direction = 1; 
    [SerializeField] int _currentPath = 0; 
    private bool _isWaiting = false;
    public Rigidbody2D _myRb;

    [Header("Estado de Cambio")]
    private bool _pendingPathChange = false;
    private int _targetPathRequest = 0;

    public Vector2 velocity; 
    private Vector2 _lastPosition;    
    
    private Vector2 _previousPosition;
    private List<Rigidbody2D> _onPlatform = new List<Rigidbody2D>();
    void Start()
    {
        transform.position = _originalWaypoints[0].position;
        _waypoints = _originalWaypoints;
        _previousPosition = _myRb.position;
        InputManager.instance.platformTest += TriggerPathChange;
    }

    void FixedUpdate()
    {
        if (_active && !_isWaiting && _waypoints.Length > 0)
        {
            MovePlatform();
        }
        else
        {            
            velocity = Vector2.zero;
        }
    }    

    void MovePlatform()
    {
        
        if (Vector2.Distance(_myRb.position, _waypoints[_currentWaypoint].position) < _maxDistance)
        {
            CheckForPathChange();
            //StartCoroutine(WaitAtWaypoint());
        }

        _previousPosition = _myRb.position;

        
        Vector2 targetPos = _waypoints[_currentWaypoint].position;
        Vector2 nextPos = Vector2.MoveTowards(_myRb.position, targetPos, _speed * Time.fixedDeltaTime);
        _myRb.MovePosition(nextPos);

        
        Vector2 deltaMovement = nextPos - _previousPosition;

        
        foreach (var rb in _onPlatform)
        {
            rb.position += deltaMovement;
        }

        if (Vector2.Distance(_myRb.position, targetPos) < _maxDistance)
        {
            CheckForPathChange();
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        _isWaiting = true;

        
        bool isEnd = (_currentWaypoint >= _waypoints.Length - 1 && _direction == 1) ||
                     (_currentWaypoint <= 0 && _direction == -1);

        if (isEnd && !_isChanging)
        {
            if (_stopWhenPathFinished)
            {
                _active = false;
                
            }
            else
            {
                _direction *= -1; 
            }
            yield return new WaitForSeconds(_waitTime);
        }
        else if (_isChanging)
        {            
            _isChanging = false;
            yield return new WaitForSeconds(_waitTime * 2);
        }
        else
        {

            if (transform.childCount > 0)
                yield return new WaitForSeconds(_waitTime * 0.5f);
        }

        
        if (_active)
        {
            _currentWaypoint += _direction;
            _currentWaypoint = Mathf.Clamp(_currentWaypoint, 0, _waypoints.Length - 1);
        }

        _isWaiting = false;
    }

    public void TriggerPathChange()
    {        
        _targetPathRequest = (_targetPathRequest == 0)? 1 : 0;
        _pendingPathChange = true;

        
        if (!_active) _active = true;
        if( _isWaiting ) _isWaiting = false;

        Debug.Log("Petición de cambio de carril recibida. Se ejecutará en el próximo cruce.");
    }

    void CheckForPathChange()
    {
        if (!_pendingPathChange) return;

        
        if (_pathIntersectionWaypoints.ContainsKey(_currentPath) &&
            _pathIntersectionWaypoints[_currentPath].ContainsKey(_currentWaypoint))
        {
            Debug.Log("Del path " + _currentPath + " y way " + _currentWaypoint + " intersecto con el way " + _pathIntersectionWaypoints[_currentPath][_currentWaypoint] + " del path " + _targetPathRequest);
            ExecutePathSwap(_targetPathRequest, _pathIntersectionWaypoints[_currentPath][_currentWaypoint]);
            _pendingPathChange = false; 
        }
    }

    void ExecutePathSwap(int newPath, int newWaypoint)
    {
        _isChanging = true;

        if (!_active) _active = true;

        if (newPath == 1) 
        {
            _waypoints = _extraWaypoints;
            _currentPath = 1;
        }
        else 
        {
            _waypoints = _originalWaypoints;
            _currentPath = 0;
        }
        
        _currentWaypoint = newWaypoint;

        Debug.Log("El current es " + _currentWaypoint);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Entity ent))
        {
            Rigidbody2D entRB = ent.entityRigidbody;
            if (!_onPlatform.Contains(entRB)) _onPlatform.Add(entRB);
            collision.transform.SetParent(this.transform);
            if (ent.TryGetComponent(out Character chara))
            {
                chara.currentPlatform = this;
            }
            //if(!_stopWhenPathFinished) _stopWhenPathFinished = true;
            if (!_active) StartCoroutine(StartActivator());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Entity ent))
        {
            Rigidbody2D entRB = ent.entityRigidbody;
            _onPlatform.Remove(entRB);
            if (ent.TryGetComponent(out Character chara))
            {
                chara.currentPlatform = null;
            }           
            
            collision.transform.SetParent(null);
        }
    }

    IEnumerator ActivateStop()
    {
        yield return new WaitForSeconds(0.7f);
        _stopWhenPathFinished = true;
    }

    IEnumerator StartActivator()
    {
        yield return new WaitForSeconds(_activationDelayTime);
        _active = true;
    }
}
