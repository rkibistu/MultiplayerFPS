using UnityEngine;
using Fusion;
using Fusion.KCC;
using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

public enum EPlayerStates {
    Idle = 0,
    Walk,
    Jump,
}

[OrderAfter(typeof(BeforeHitboxManagerUpdater))]
[OrderBefore(typeof(KCC), typeof(HitboxManager), typeof(AfterHitboxManagerUpdater))]
[RequireComponent(typeof(KCC), typeof(BeforeHitboxManagerUpdater), typeof(AfterHitboxManagerUpdater))]
[RequireComponent(typeof(Weapons), typeof(Health))]
[RequireComponent(typeof(AnimationController), typeof(Animator))]
public class AgentStateMachine : NetworkKCCProcessor, IBeforeAllTicks {


    public void BeforeAllTicks(bool res, int x) {

        //     Called before the resimulation loop (when applicable), and also before the forward
        //     simulation loop. Only called on Updates where resimulation or forward ticks are
        //     processed.

        // avem un enum [Networked] care tine minte starea curenta (pentru a nu avea o clasa de tip BaseState [Networked], asta ar creste bandwith-ul tare)
        // In functia asta setam mereu starea curenta in functie de acel enum
        // Avem garantia ca functia asta se executa mereu ianinte de resimulation.foward loop


        // sincronizeaza starea curenta cu starea din tick-ul pentru care se face simularea
        SyncLocalCurrentState();
    }

    // PUBLIC MEMBERS

    public RoomPlayer Owner { get; set; }
    public KCC KCC => _kcc;

    public event Action<AgentStateMachine> AgentDespawned;
    public Weapons Weapons => _weapons;

    public PickupItem PickupItem => _pickupItem;
    public Health Health => _health;

    public AgentVisual Visual => _visual;

    public AnimationController Animator => _animator;

    public GameplaySceneController _gameplaySceneController;

    // PRIVATE MEMBERS
    private KCC _kcc;
    private GameObject _mainCamera;
    private Weapons _weapons;
    private PickupItem _pickupItem;
    private Health _health;
    private AnimationController _animator;

    private bool _kccHasPositionToSet = false;
    private Vector3 _kccPositionToSet;

   

    [Networked]
    public EPlayerStates CurrentState { get; set; }

    private BaseState _currentState;
    private BaseState _oldState;

    [SerializeField]
    private Transform _cameraPivot;
    [SerializeField]
    private Transform _cameraHandle;


    private AgentVisual _visual;

    // PROTECTED MEMEBRS

    public GameObject MainCamera => _mainCamera;

    public Transform CameraPivot => _cameraPivot;
    public Transform CameraHandle => _cameraHandle;


    public Idle _idleState;
    public Walk _walkState;
    public Jump _jumpState;

    public int x = 0;

    // NetworkBehaviour INTERFACE

    public override sealed void Spawned() {

        // Explicit KCC initialization. This needs to be called before using API, otherwise changes could be overriden by implicit initialization from KCC.Start() or KCC.Spawned()
        _kcc.Initialize(EKCCDriver.Fusion);

        // Player itself can modify kinematic speed, registering to KCC
        _kcc.AddModifier(this);

        // KCC is updated manually to preserve correct execution order
        _kcc.SetManualUpdate(true);


        _currentState = _idleState;
        _oldState = _currentState;

        _currentState.OnSpawned();

        Weapons.OnSpawned();
    }

    public override sealed void Despawned(NetworkRunner runner, bool hasState) {

        Weapons.OnDespawned();

        AgentDespawned?.Invoke(this);
        AgentDespawned = null;

        _currentState.OnDespawned();
    }

    /// <summary>
    /// 2. Regular fixed update for agents
    /// </summary>
    public override sealed void FixedUpdateNetwork() {
        // Hitbox manager saves positions in forward frame so it is enough to interpolate in forward
        if (Runner.Stage == SimulationStages.Forward && IsProxy == true && _kcc.enabled == true) {
            // Interpolate proxies early before HitboxManager updates
            _kcc.Interpolate();
        }

        // At this point all agents (including proxies) have set their positions and rotations, we can run some post-processing (setting camera pivots, synchronizing other owned objects, ...).


        // Daca s-a icnercat o mutare a playerului folsoind functia MoveTo(), acum se realizeaza
        DoMoveTo();

        // CurrentState nu e [Networked} variable, deci nu se reseteaza la valorile precedente in cazul resimularilor
        // OBLIGATORIU: inainte de a folosit orice functie din CurrentState, trebuie setata starea in functie de enum-ul [Networked]
        _currentState.OnFixedUpdate();
    }

    /// <summary>
    /// 5. Regular render update for Agent
    /// </summary>
    public override sealed void Render() {
        // At this point all agents have set their positions and rotations, we can run some post-processing (setting camera pivots, synchronizing other owned objects, ...).

        _currentState.OnRender();
    }


    public void ChangeState(EPlayerStates newState) {

        if (_currentState != GetState(newState)) {

            _currentState.Exit();

            _oldState = _currentState;
            _currentState = GetState(newState);
            CurrentState = newState;

            _currentState.Enter();
        }
    }

    // OBLIGATORIU: se apeleaza inainte de a apela orice functie a starii curente (_currentState.orice)
    //      _currentState nu e [Networked] -> nu e resetata de fusion la resimulari
    //      CurrentState este [Networked] -> asigura ca resimualrile sa se faca avadn starea curenta setata corect
    public void SyncLocalCurrentState() {

        if (_currentState != GetState(CurrentState)) {

            ChangeState(CurrentState);
        }
    }

    //Cere sa se schimbe starea actuala
    // modifica doar variabila CurrentState de tip enum EPlayerStates
    // modificarea efectiva se face la apelarea functiei ChangeState()




    // PROTECTED METHODS

    // Aplica schimbarile setate in functia public MoveTo
    // functia aceasta trebuie neaparat apela in FixedUpdate(Network)
    protected void DoMoveTo() {

        if (_kccHasPositionToSet == false)
            return;

        Debug.Log("kcc set position: " + _kccPositionToSet);
        KCC.SetPosition(_kccPositionToSet);
        _kccHasPositionToSet = false;
    }

    // MONOBEHAVIOUR

    protected virtual void Awake() {


        _kcc = GetComponent<KCC>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (_mainCamera == null)
            Debug.LogError("Main Camera is missing from the scene!");


        _weapons = GetComponent<Weapons>();
        _pickupItem = GetComponent<PickupItem>();
        _health = GetComponent<Health>();
        _animator = GetComponent<AnimationController>();
        _visual = GetComponent<AgentVisual>();
        // All agents have BeforeHitboxManagerUpdater and AfterHitboxManagerUpdater component.

        // BeforeHitboxManagerUpdater provides callbacks which are executed before HitboxManager => we use this to process "movement" input - set move direction, jump, look rotation, ...

        var beforeUpdater = GetComponent<BeforeHitboxManagerUpdater>();
        beforeUpdater.SetDelegates(EarlyFixedUpdate, EarlyRender);

        // AfterHitboxManagerUpdater provides callbacks which are executed after HitboxManager => we use this to process "non-movement" input - shooting, actions, ...

        var afterUpdater = GetComponent<AfterHitboxManagerUpdater>();
        afterUpdater.SetDelegates(LateFixedUpdate, LateRender);

        _idleState = new Idle(this);
        _walkState = new Walk(this);
        _jumpState = new Jump(this);
    }

    // PUBLIC METHODS

    // KCC.SetPosition trebuie apleata din FixedUpdateNetwork pentru a mentrine modificarile
    // aceasta functie e apelata oricand pentru a pregati tereneul, astfel incat in primul FixedUpdateNetwork sa se seteze pozitia
    public void MoveTo(Vector3 position) {

        //KCC.SetPosition(position);

        _kccHasPositionToSet = true;
        _kccPositionToSet = position;
    }

    // PRIVATE METHODS

    /// <summary>
    /// 1. At this point new input is gathered so process movement part of it before updating positions in HitboxManager
    /// </summary>
    private void EarlyFixedUpdate() {
        // This method expects derived classes to make movement / look related calls to KCC.
        _currentState.ProcessEarlyFixedInput();

        // All movement related properties set, we can trigger manual KCC update.
        if (_kcc.enabled == true) {

            _kcc.ManualFixedUpdate();
        }

        // This method can be used to post-process KCC update (Transform is already updated as well).
        // This is executed before any of Agent and HitboxManager FixedUpdateNetwork().
        _currentState.OnEarlyFixedUpdate();
    }

    /// <summary>
    /// 3. Executed after all Agent and HitboxManager FixedUpdateNetwork() calls, process rest of player input (shooting, other non-movement related actions).
    /// </summary>
    private void LateFixedUpdate() {
        if (IsProxy == false) {
            _currentState.ProcessLateFixedInput();
        }

        // This method can be used to react on player actions. At this point player input has been processed completely.
        _currentState.OnLateFixedUpdate();
    }

    /// <summary>
    /// 4. Process input for render update. Only input and state authority will make changes, proxies are already interpolated.
    /// </summary>
    private void EarlyRender() {
        if (HasInputAuthority == true) {
            // This method expects derived classes to make movement / look related calls to KCC.
            _currentState.ProcessRenderInput();
        }

        // All movement related properties set, we can trigger manual KCC update.
        if (_kcc.enabled == true) {
            _kcc.ManualRenderUpdate();
        }

        // This method can be used to post-process KCC update (Transform is already updated as well).
        // This is executed before any of Agent Render().
        _currentState.OnEarlyRender();
    }

    /// <summary>
    /// 6. Executed after all Agent Render() calls
    /// </summary>
    private void LateRender() {
        // Here comes "late" render input processing of all other non-movement actions.
        // This gives you extra responsivity at the cost of maintaining extrapolation and reconcilliation.
        // Currently there are no specific actions extrapolated for render.

        // This method can be used to override final state of the object for render. At this point player input has been processed completely.
        _currentState.OnLateRender();
    }

    //KCC
    //public override EKCCStages GetValidStages(KCC kcc, KCCData data) {
    //    // Only SetKinematicSpeed stage is used, rest are filtered out and corresponding method calls will be skipped.
    //    return EKCCStages.SetKinematicSpeed;
    //}
    //public override void SetKinematicSpeed(KCC kcc, KCCData data) {
    //    // Applying multiplier.

    //    data.KinematicSpeed = 500;


    //}


    private BaseState GetState(EPlayerStates state) {

        switch (state) {
            case EPlayerStates.Idle:
                return _idleState;
            case EPlayerStates.Jump:
                return _jumpState;
            case EPlayerStates.Walk:
                return _walkState;
            default:
                return null;
        }
    }

}
