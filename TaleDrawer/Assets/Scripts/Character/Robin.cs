using UnityEngine;
using DG.Tweening;
public class Robin : Character
{
    public Sequence currentTween;
    public Subibaja subibaja;
    public bool canClimb;
    public GameObject boleadorasPrefab;
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
        };

        Boleadoras.OnUpdate += () =>
        {
            

        };
        Boleadoras.OnFixedUpdate += () =>
        {

        };

        Boleadoras.OnExit += x =>
        {

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
        var ln = GetComponent<LineRenderer>();

        for (int i = 0; i < ln.positionCount; i++)
        {
            ln.SetPosition(i, transform.position);
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
