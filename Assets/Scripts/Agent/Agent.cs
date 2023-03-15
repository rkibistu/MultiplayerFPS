using Fusion;
using Fusion.KCC;
using System;
using UnityEngine;


[OrderAfter(typeof(BeforeHitboxManagerUpdater))]
[OrderBefore(typeof(KCC), typeof(HitboxManager), typeof(AfterHitboxManagerUpdater))]
[RequireComponent(typeof(KCC), typeof(BeforeHitboxManagerUpdater), typeof(AfterHitboxManagerUpdater))]
[RequireComponent(typeof(Weapons), typeof(Health))]
public abstract class Agent : NetworkKCCProcessor {


    // PUBLIC MEMBERS
    public RoomPlayer Owner { get; set; }
  

    public KCC KCC => _kcc;
    public AgentVisual Visual => _visual;


    // PRIVATE MEMBERS


    [SerializeField]
    private Vector3 _jumpImpulse = new Vector3(0f, 6f, 0f);
    [SerializeField]
    private Transform _cameraPivot;
    [SerializeField]
    private Transform _cameraHandle;
    [SerializeField]
    private float _maxCameraAngle = 75f;


    private KCC _kcc;
    private AgentVisual _visual;
   

    // PROTECTED MEMBERS
    protected Transform CameraPivot => _cameraPivot;
    protected Transform CameraHandle => _cameraHandle;
    protected float MaxCameraAngle => _maxCameraAngle;
    protected Vector3 JumpImpulse => _jumpImpulse;


    // NetworkBehaviour INTERFACE

    public override sealed void Spawned() {

        // Explicit KCC initialization. This needs to be called before using API, otherwise changes could be overriden by implicit initialization from KCC.Start() or KCC.Spawned()
        _kcc.Initialize(EKCCDriver.Fusion);

        // Player itself can modify kinematic speed, registering to KCC
        _kcc.AddModifier(this);

        // KCC is updated manually to preserve correct execution order
        _kcc.SetManualUpdate(true);

        OnSpawned();
    }

    public override sealed void Despawned(NetworkRunner runner, bool hasState) {
        OnDespawned();
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

        OnFixedUpdate();
    }

    /// <summary>
    /// 5. Regular render update for Agent
    /// </summary>
    public override sealed void Render() {
        // At this point all agents have set their positions and rotations, we can run some post-processing (setting camera pivots, synchronizing other owned objects, ...).

        OnRender();
    }

    protected abstract void ProcessEarlyFixedInput();
    protected abstract void ProcessLateFixedInput();
    protected abstract void ProcessRenderInput();

    protected virtual void OnSpawned() { }
    protected virtual void OnDespawned() { }
    protected virtual void OnEarlyFixedUpdate() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnLateFixedUpdate() { }
    protected virtual void OnEarlyRender() { }
    protected virtual void OnRender() { }
    protected virtual void OnLateRender() { }


    // MONOBEHAVIOUR

    protected virtual void Awake() {


        _kcc = GetComponent<KCC>(); 
        _visual = gameObject.GetComponent<AgentVisual>();

   

        // All agents have BeforeHitboxManagerUpdater and AfterHitboxManagerUpdater component.

        // BeforeHitboxManagerUpdater provides callbacks which are executed before HitboxManager => we use this to process "movement" input - set move direction, jump, look rotation, ...

        var beforeUpdater = GetComponent<BeforeHitboxManagerUpdater>();
        beforeUpdater.SetDelegates(EarlyFixedUpdate, EarlyRender);

        // AfterHitboxManagerUpdater provides callbacks which are executed after HitboxManager => we use this to process "non-movement" input - shooting, actions, ...

        var afterUpdater = GetComponent<AfterHitboxManagerUpdater>();
        afterUpdater.SetDelegates(LateFixedUpdate, LateRender);
    }

    // PRIVATE METHODS

    // Next 4 methods are called in Before/After HitboxManager (see Awake method)

    /// <summary>
    /// 1. At this point new input is gathered so process movement part of it before updating positions in HitboxManager
    /// </summary> 
    private void EarlyFixedUpdate() {
        // This method expects derived classes to make movement / look related calls to KCC.
        ProcessEarlyFixedInput();

        // All movement related properties set, we can trigger manual KCC update.
        if (_kcc.enabled == true) {      
           
            _kcc.ManualFixedUpdate();
        }

        // This method can be used to post-process KCC update (Transform is already updated as well).
        // This is executed before any of Agent and HitboxManager FixedUpdateNetwork().
        OnEarlyFixedUpdate();
    }

    /// <summary>
    /// 3. Executed after all Agent and HitboxManager FixedUpdateNetwork() calls, process rest of player input (shooting, other non-movement related actions).
    /// </summary>
    private void LateFixedUpdate() {
        if (IsProxy == false) {
            ProcessLateFixedInput();
        }

        // This method can be used to react on player actions. At this point player input has been processed completely.
        OnLateFixedUpdate();
    }

    /// <summary>
    /// 4. Process input for render update. Only input and state authority will make changes, proxies are already interpolated.
    /// </summary>
    private void EarlyRender() {
        if (HasInputAuthority == true) {
            // This method expects derived classes to make movement / look related calls to KCC.
            ProcessRenderInput();
        }

        // All movement related properties set, we can trigger manual KCC update.
        if (_kcc.enabled == true) {
            _kcc.ManualRenderUpdate();
        }

        // This method can be used to post-process KCC update (Transform is already updated as well).
        // This is executed before any of Agent Render().
        OnEarlyRender();
    }

    /// <summary>
    /// 6. Executed after all Agent Render() calls
    /// </summary>
    private void LateRender() {
        // Here comes "late" render input processing of all other non-movement actions.
        // This gives you extra responsivity at the cost of maintaining extrapolation and reconcilliation.
        // Currently there are no specific actions extrapolated for render.

        // This method can be used to override final state of the object for render. At this point player input has been processed completely.
        OnLateRender();
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

}
