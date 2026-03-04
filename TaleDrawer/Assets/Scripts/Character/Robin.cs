using UnityEngine;
using DG.Tweening;
public class Robin : Character
{
    public Sequence currentTween;
    public Subibaja subibaja;
    public bool canClimb;
    public GameObject boleadorasPrefab;

    public LineRenderer lineRenderer;

    protected override void Awake()
    {
        characterModel = new CharacterModel(this, characterRigidbody, floorLayerMask);
        characterView = new CharacterView(this, _animator, _characterSprite);
        base.Awake();

        #region State Declaration

        var OnRope = new StateE<CharacterStates>("OnRope");
        var JumpingToRope = new StateE<CharacterStates>("JumpingToRope");
        var Swaying = new StateE<CharacterStates>("Swaying");
        var Boleadoras = new StateE<CharacterStates>("Boleadoras");

        #endregion

        #region State Configuration

        var OnRopeC = StateConfigurer.Create(OnRope);

        OnRopeC.SetTransition(CharacterStates.Moving, states[CharacterStates.Moving].state)
            .SetTransition(CharacterStates.Wait, states[CharacterStates.Wait].state)
            .SetTransition(CharacterStates.Stop, states[CharacterStates.Stop].state)
            .SetTransition(CharacterStates.JumpingToRope, JumpingToRope)
            .SetTransition(CharacterStates.Idle, states[CharacterStates.Idle].state).Done();

        states.Add(CharacterStates.OnRope, new StateDefinition(OnRope, OnRopeC));

        var JumpingToRopeC = StateConfigurer.Create(JumpingToRope);

        JumpingToRopeC.SetTransition(CharacterStates.Swaying, Swaying)
            .SetTransition(CharacterStates.Moving, states[CharacterStates.Moving].state).Done();

        states.Add(CharacterStates.JumpingToRope, new StateDefinition(JumpingToRope, JumpingToRopeC));

        var SwayingC = StateConfigurer.Create(Swaying);

        SwayingC.SetTransition(CharacterStates.JumpingToRope, JumpingToRope).Done();

        states.Add(CharacterStates.Swaying, new StateDefinition(Swaying, SwayingC));

        var BoleadorasC = StateConfigurer.Create(Boleadoras);

        BoleadorasC.SetTransition(CharacterStates.Idle, states[CharacterStates.Idle].state).Done();

        states.Add(CharacterStates.Boleadoras, new StateDefinition(Boleadoras, BoleadorasC));

        states[CharacterStates.Wait].stateConfigurer.SetTransition(CharacterStates.OnRope, OnRope).Done();
        states[CharacterStates.Moving].stateConfigurer.SetTransition(CharacterStates.Swaying, Swaying).SetTransition(CharacterStates.Boleadoras, Boleadoras).Done();
        states[CharacterStates.Idle].stateConfigurer.SetTransition(CharacterStates.Boleadoras, Boleadoras).Done();
        #endregion

        #region ONROPE STATE

        OnRope.OnEnter += x =>
        {

            _currentState = CharacterStates.OnRope;


        };

        OnRope.OnUpdate += () =>
        {


        };
        OnRope.OnFixedUpdate += () =>
        {

        };
        OnRope.OnExit += x => { };

        #endregion



        #region JUMPINGTOROPE STATE

        JumpingToRope.OnEnter += x =>
        {
            _currentState = CharacterStates.JumpingToRope;
        };

        JumpingToRope.OnUpdate += () =>
        {

            if (grounded)
            {
                SendInputToFSM(CharacterStates.Moving);
            }
        };
        JumpingToRope.OnFixedUpdate += () =>
        {

        };

        JumpingToRope.OnExit += x =>
        {

        };

        #endregion

        #region SWAYING STATE

        Swaying.OnEnter += x =>
        {


            _currentState = CharacterStates.Swaying;
        };

        Swaying.OnUpdate += () =>
        {


        };
        Swaying.OnFixedUpdate += () =>
        {

        };

        Swaying.OnExit += x =>
        {

        };

        #endregion


        #region BOLEADORAS STATE

        Boleadoras.OnEnter += x =>
        {

            _currentState = CharacterStates.Boleadoras;
            currentSpeed = currentSpeed / 3;
        };

        Boleadoras.OnUpdate += () =>
        {
            characterModel.AlignToGround();


        };
        Boleadoras.OnFixedUpdate += () =>
        {
            if (xInput != 0)
            {
                characterModel.Move2(xInput);
            }
            else
            {
                characterRigidbody.linearVelocityX = 0;

              //  SendInputToFSM(CharacterStates.Idle);
            }

        };

        Boleadoras.OnExit += x =>
        {
            currentSpeed = maxSpeed;
        };

        #endregion

    }
    protected override void Start()
    {
        base.Start();

    }

    public override void Update()
    {
        base.Update();

        
    }

    public override void CatchBoleadoras()
    {
        base.CatchBoleadoras();
        hasObject = true;
        SendInputToFSM(CharacterStates.Boleadoras);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
   
        
        if(collision.TryGetComponent(out IInteractableP inter) && inter.MyInteractableType() == InteractableType.ClimbingObj)
        {
            canClimb = true;
         //   currentInteractable = inter;
            
        }

    }

    public override void Shoot(Vector2 force)
    {
        base.Shoot(force);
        var bol = Instantiate(boleadorasPrefab);
        bol.transform.position = transform.position;
        bol.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        SendInputToFSM(CharacterStates.Idle);
        hasObject = false;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, transform.position);
        }
    }

    public override void TrayectoryVisuals(Vector2 dir)
    {

        float xComponent = 0;
        float yComponent = 0;
        float maxTime = 1f;
        int steps = 30;
        float timeStep = maxTime / steps;
        int linerendererIndex = 0;

        for (int i = 0; i < 30; i++)
        {
            float t = i * timeStep;
            xComponent = transform.position.x + dir.x * t;
            yComponent = transform.position.y + dir.y * t - 0.5f * 9.8f * 2f * Mathf.Pow(t, 2);
            lineRenderer.SetPosition(linerendererIndex, new Vector3(xComponent, yComponent, 0));
            linerendererIndex++;
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (collision.TryGetComponent(out IInteractableSP inter) && inter.MyInteractableType() == InteractableType.ClimbingObj)
        {
            canClimb = false;            
        }
    }


}
