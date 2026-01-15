using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Collections;
public class Caja : SpawningObject,IInteractable
{
    
    [SerializeField] LayerMask _spMask;
    [SerializeField] Transform _playerPos;
    [SerializeField] LayerMask _eventMask;
    [SerializeField] LayerMask _excludeLayers;
    [SerializeField] Vector2 _jumpPosition;
    [SerializeField] LayerMask _clickable;

    [SerializeField] bool canInteract = true;

    [SerializeField] Collider2D _upperCollider;
    [SerializeField] PlatformEffector2D effector2D;
    [SerializeField] NewSerializableDictionary<float, Collider2D> _colliders;
    [SerializeField] InteractableType _interactableType;

    public bool isMoving;
    public override void Start()
    {
        base.Start();
        StartCoroutine(CheckSoil());
        _upperCollider.forceReceiveLayers = _excludeLayers;


    }
    private void Update()
    {
        if(_spawned && _myrb.angularVelocity != 0 && !isMoving)
        {
            isMoving = true;
        }
        else if(_spawned && _myrb.angularVelocity == 0 && isMoving)
        {
            isMoving = false;
            foreach (var item in _colliders)
            {
                if(Mathf.Abs(item.Key - transform.eulerAngles.z) < 1f)
                {
                    item.Value.enabled = true;
                    effector2D.rotationalOffset = -transform.eulerAngles.z;
                }
                else
                {
                    item.Value.enabled = false;
                }
            }

        }
    }
    private void OnDestroy()
    {
        interactableDelegate?.Invoke(-weight,gameObject);
    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
    }
    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
       
    }

    public void Interact(SpawningObject spawningObject)
    {
       
    }

    public void Interact(GameObject interactor)
    {
        
    }

    public override void Paint()
    {
        StartCoroutine(GetComponentInChildren<Paint>().PaintSprite());
    }
    public void InteractWithPlayer()
    {
     
       if(Physics2D.OverlapCircle(transform.position,transform.localScale.x, _clickable) && canInteract)
        {
           _excludeLayers = _myColl.excludeLayers ;
             Vector2 newPos = _myColl.bounds.ClosestPoint(_myCharacter.transform.position);
            newPos.y = transform.position.y;
            float dist = Vector3.Distance(transform.position, _myCharacter.transform.position);
            Debug.Log("La distancia con la caja es: " + dist);
            if(_myCharacter.transform.position.x < transform.position.x)
            {
                if(dist < 1f)
                {
                    newPos -= Vector2.right * 1.35f;
                }
                else
                {
                    newPos -= Vector2.right * .6f;
                }
                
            }
            else
            {
                if (dist < 1f)
                {
                    newPos += Vector2.right * 1.35f;
                }
                else
                {
                    newPos += Vector2.right * .6f;
                }
           
            }
            Debug.Log(newPos);
          //  newPos.y = transform.position.y;

            _myCharacter.SendInputToFSM(CharacterStates.Moving);

        }
    }

    public void JumpOverBox()
    {
        float sign = Mathf.Sign(_myCharacter.transform.position.x - transform.position.x);
        if (_myCharacter.flipSign == sign)
        {
            _myCharacter.characterView.FlipCharacter(Mathf.RoundToInt(-sign));

        }

        _myCharacter.currentInteractable = this;
   //     _myColl.excludeLayers = default;
        _myCharacter.SendInputToFSM(CharacterStates.Climb);
        _myCharacter.StartCoroutine(RunToCenter());

    }
    public void JumpOffBox()
    {
        Vector2 touchPos = GameManager.instance._sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        var hit2 = Physics2D.OverlapCircle(touchPos, .2f, _clickable);
        if (hit2 == null) return;
        _myCharacter.currentInteractable = null;
        _myColl.excludeLayers = _excludeLayers;
        var eventObj = Physics2D.OverlapBox(transform.position,new Vector2(2,2),0,_eventMask);

        BoxEvent boxE = default;
        if(eventObj!= null && eventObj.TryGetComponent(out BoxEvent boxEvent))
        {
            _jumpPosition = CustomTools.ToVector2(boxEvent.jumpPosition.position);
            boxE = boxEvent;
        }
        if (_jumpPosition != Vector2.zero)
        {

            if(Mathf.Sign(transform.position.x - _jumpPosition.x) * Mathf.Sign(transform.position.x - touchPos.x)==1)
            {
                if (boxE.shouldClimb)
                {
                    _myCharacter.characterRigidbody.gravityScale = 0;
                    _myCharacter.climbAction = () =>
                    {
                        _myCharacter.SendInputToFSM(CharacterStates.Moving);
                        _myCharacter.characterRigidbody.gravityScale = 1;
                        _myCharacter.climbAction = null;
                    };
                    _myCharacter.characterModel.Jump(
                        _jumpPosition,
                        () =>
                        {
                            _myCharacter.SendInputToFSM(CharacterStates.Climb);
                        },
                        true,   // tercer parámetro: toJumpingState
                        0.5f    // cuarto parámetro: time
                    );
                }
                else
                {
                    _myCharacter.characterModel.Jump(_jumpPosition, () =>
                    {

                       

                    });
                }

            }
            else
            {
                if (touchPos.x > transform.position.x)
                {
                    _myCharacter.characterModel.Jump(transform.position + Vector3.right * 2, () =>
                    {

                        

                    });
                }
                else
                {
                    _myCharacter.characterModel.Jump(transform.position - Vector3.right * 2, () =>
                    {
                        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                   

                    });

                }
            }
        }
        else
        {
          
            if (touchPos.x > transform.position.x)
            {
                _myCharacter.characterModel.Jump(transform.position + Vector3.right * 2, () =>
                {
                 
                    

                });
            }
            else
            {
                _myCharacter.characterModel.Jump(transform.position - Vector3.right * 2, () =>
                {

                  

                });

            }
        }

        

    }
    public IEnumerator RunToCenter()
    {
        yield return new WaitForSeconds(.5f);
        _myColl.excludeLayers = default;
        yield return new WaitForSeconds(.4f);
        _myCharacter.characterView.OnMove();
        _myCharacter.transform.DOMove(new Vector2(transform.position.x, _myCharacter.transform.position.y), .3f);
        yield return new WaitForSeconds(.3f);
        _myCharacter.characterView.OnIdle();
       _myCharacter.SendInputToFSM(CharacterStates.Wait);

    }

    IEnumerator CheckSoil()
    {
        Collider2D col = GetComponent<Collider2D>();

        while (true)
        {
            yield return new WaitForSeconds(.2f);

            Vector2 origin = col.bounds.center;

            Vector2 downOrigin = origin;
            downOrigin.y = col.bounds.min.y - 0.01f;

            Vector2 upOrigin = origin;
            upOrigin.y = col.bounds.max.y + 0.01f;

            RaycastHit2D hitDown = Physics2D.Raycast(downOrigin, Vector2.down, 1f, _spMask);
            RaycastHit2D hitUp = Physics2D.Raycast(upOrigin, Vector2.up, 1f, _spMask);

            Debug.DrawRay(downOrigin, Vector2.down, Color.red);
            Debug.DrawRay(upOrigin, Vector2.up, Color.blue);

            bool canDown = IsValidHit(hitDown);
            bool canUp = IsValidHit(hitUp);

            canInteract = canDown && canUp;
        }
    }

    bool IsValidHit(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return true;

        // Ignorar triggers
        if (hit.collider.isTrigger)
            return true;

        // Ignorar el propio objeto
        if (hit.collider.gameObject == gameObject)
            return true;

        // Ignorar objetos del mismo tipo
        return !hit.collider.TryGetComponent(out SpawningObject _);
    }
    public override void Delete()
    {
        base.Delete();
        if(Character.instance.currentInteractable == (IInteractable)this)
        {
            Character.instance.currentInteractable = null;
        }
        Destroy(gameObject);
    }
    public void InsideInteraction()
    {
        
        JumpOffBox();
    }
}
