using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.KCC;
using System;
using Fusion;

/*
    Main state of the player. Toate celeltate stari au aceasta stare ca parinte!
    Aici avem functionalitati independente de starea curenta (se intampla mereu)
    Aici declaram membrii prezenti in toate starile.
 */

[Serializable]
public abstract class PlayerMainState : BaseState {
    [SerializeField]
    protected float _maxCameraAngle = 75f;


    private bool _kccHasPositionToSet = false;
    private Vector3 _kccPositionToSet;


    public PlayerMainState(string name, AgentStateMachine stateMachine) : base(name, stateMachine) {

    }

    public override void OnSpawned() {
        base.OnSpawned();

        _agentStateMachine.Health.FatalHitTaken += OnFatalHitTaken;

        if (_agentStateMachine.HasInputAuthority)
            _agentStateMachine.Visual.SetVisibility(false);
    }

    public override void OnDespawned() {
        base.OnDespawned();

        _agentStateMachine.Health.FatalHitTaken -= OnFatalHitTaken;
    }

    public override void ProcessEarlyFixedInput() {
        // Settings for camera movement
        if (_agentStateMachine.Owner == null) {
            GameManager.Instance.SetOwnersForAllAgents();
        }

        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;

        MyDebug.Log("");
        MyDebug.Log(_name + " Process Early Fixed Input  ");

        var input = _agentStateMachine.Owner.Input.FixedInput;

        //Daca s-a facut in request de schimbare a pozitiei(teleportare) -> muta acum playerul
        DoMoveTo();

        // Clamp input look rotation delta
        Vector2 lookRotation = _agentStateMachine.KCC.FixedData.GetLookRotation(true, true);
        Vector2 lookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, input.LookRotationDelta, -_maxCameraAngle, _maxCameraAngle);

        // Apply clamped look rotation delta
        _agentStateMachine.KCC.AddLookRotation(lookRotationDelta);

    
        // Cast a ray. Check if hit pickubleItem. Pickup it
        TryPickupItem();
    }


    public override void OnEarlyFixedUpdate() {
        base.OnEarlyFixedUpdate();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
        MyDebug.Log(_name + " On Early Fixed Update  ");

    }

    public override void OnFixedUpdate() {
        base.OnFixedUpdate();

        // Settings for camera movement

        // Regular fixed update for Agent class.
        // Executed after all agent KCC updates and before HitboxManager.
        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;
        MyDebug.Log(_name + " On Fixed Update  ");

        // Setting camera pivot rotation
        Vector2 pitchRotation = _agentStateMachine.KCC.FixedData.GetLookRotation(true, false);
        _agentStateMachine.CameraPivot.localRotation = Quaternion.Euler(pitchRotation);


    }

    public override void ProcessLateFixedInput() {

        // Executed after HitboxManager. Process other non-movement actions like shooting.
        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
        MyDebug.Log(_name + " Process Late Fixed Input   ");

    }

    public override void OnLateFixedUpdate() {
        base.OnLateFixedUpdate();

        MyDebug.Log(_name + " On Late Fixed Update  ");

        // Update all projectiles that are already Spawned (calcule)
        _agentStateMachine.Weapons.OnLateFixedUpdate();
    }

    public override void ProcessRenderInput() {
        // Settings for camera movement
        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;
        MyDebug.Log(_name + " Process Render Input  ");

        var input = _agentStateMachine.Owner.Input;

        // Get look rotation from last fixed update (not last render!)
        Vector2 lookRotation = _agentStateMachine.KCC.FixedData.GetLookRotation(true, true);

        // For correct look rotation, we have to apply deltas from all render frames since last fixed update => stored in Input.CachedInput
        Vector2 lookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, input.CachedInput.LookRotationDelta, -_maxCameraAngle, _maxCameraAngle);

        _agentStateMachine.KCC.SetLookRotation(lookRotation + lookRotationDelta);
    }

    public override void OnEarlyRender() {
        base.OnEarlyRender();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
        MyDebug.Log(_name + " On Early Render ");

    }

    public override void OnRender() {
        base.OnRender();

        MyDebug.Log(_name + " On Render  ");

        // Randeaza proiectilele deja existente
        _agentStateMachine.Weapons.OnRender();
    }

    public override void OnLateRender() {
        base.OnLateRender();
        // Setting base camera transform based on handle

        // Trebuie asta?
        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
        MyDebug.Log(_name + " On Late Render  ");


        // For render we care only about input authority.
        // This can be extended to state authority if needed (inner code won't be executed on host for other agents, having camera pivots to be set only from fixed update, causing jitter if spectating that player)
        if (_agentStateMachine.HasInputAuthority == true) {
            Vector2 pitchRotation = _agentStateMachine.KCC.RenderData.GetLookRotation(true, false);
            _agentStateMachine.CameraPivot.localRotation = Quaternion.Euler(pitchRotation);

            var cameraTransform = _agentStateMachine.MainCamera.transform; //ATENTIE AICI! DACA NU SE MISCA CAMERA, APPLICA DIRECT PE MAIN CAMERA MODIFICARILE!

            cameraTransform.position = _agentStateMachine.CameraHandle.position;
            cameraTransform.rotation = _agentStateMachine.CameraHandle.rotation;
        }
    }

    public override void Enter() {
        base.Enter();
        // Reapeleaza processEarlyFixedInput
        // In ProcessEarlyFixedInput se face schimbarea dintr-o stare in alta.
        //      Vrem ca atunci cand intram intr-o noua stare, sa apelam si pentru aceasta ProcessEarlyFixedInput
        // Daca nu am apela aici, noua stare ar apela toate functiile cu exceptia ProcessEarlyFixedInput.
        //Flow: Idle.ProcessEarlyFixedInput() -> change state -> NewState.Enter() -> NewState.ProcessEarlyFixedInput()
        MyDebug.Log(_name + " Enter  ");


        //if (_agentStateMachine.HasInputAuthority || Context.Instance.Runner.IsServer) {


        //    int tick = Context.Instance.Runner.Tick;
        //    if (Context.Instance.Runner.IsForward) {

        //        Debug.Log("Forward enter: " + _name + " " + _agentStateMachine.CurrentState + " " + tick + " id:" + _agentStateMachine.Owner.Id);
        //    }
        //    else {
        //        Debug.Log("Resimulation enter: " + _name + " " + _agentStateMachine.CurrentState + " " + tick + " id:" + _agentStateMachine.Owner.Id);
        //    }
        //}

        ProcessEarlyFixedInput();
    }
    public override void Exit() {
        base.Exit();

        MyDebug.Log(_name + " Exit  ");

    }



    // KCC.SetPosition trebuie apleata din FixedUpdateNetwork pentru a mentrine modificarile
    // aceasta functie e apelata oricand pentru a pregati tereneul, astfel incat in primul FixedUpdateNetwork sa se seteze pozitia
    public void MoveTo(Vector3 position) {

        //KCC.SetPosition(position);

        _kccHasPositionToSet = true;
        _kccPositionToSet = position;
    }
    // Aplica schimbarile setate in functia public MoveTo
    // functia aceasta trebuie neaparat apela in FixedUpdate(Network)
    protected void DoMoveTo() {

        if (_kccHasPositionToSet == false)
            return;

        _agentStateMachine.KCC.SetPosition(_kccPositionToSet);
        _kccHasPositionToSet = false;
    }



    private void OnFatalHitTaken(HitData hitData) {

        RoomPlayer instigator = Context.Instance.Gameplay.Players[hitData.InstigatorRef];
        AgentStateMachine targetAgent = hitData.Target.GameObject.GetComponent<AgentStateMachine>();

        if (targetAgent.Owner == null) {

            GameManager.Instance.SetOwnersForAllAgents();
        }

        RoomPlayer target = targetAgent.Owner;

        instigator.PlayerScore.IncrementKills();
        target.PlayerScore.IncrementDeaths();

        
        _agentStateMachine._gameplaySceneController.AnnounceKill(hitData, _agentStateMachine.Weapons.CurrentWeapon.Icon );

        // PLAY Dead ANIMATION
        _agentStateMachine.Animator.Play(_agentStateMachine.Animator.DeathFlyingBack);
    }

    private void TryPickupItem() {

        if (_agentStateMachine.Owner.Input.WasPressed(EInputButtons.Test) == true) {


            PickableItem item = _agentStateMachine.PickupItem.FindItem(_agentStateMachine.MainCamera.GetComponent<Camera>());
            if (item != null) {


                EquipablePickable equipable = item.GetComponent<EquipablePickable>();
                if (equipable) {
                    Debug.Log("Equip weapon. Call Rpc");
                    _agentStateMachine.Weapons.AddWeapon_RPC(equipable.GetPrefabAndDestroy().GetComponent<WeaponIdentifier>()._weaponIdentifier);
                }

                ConsumablePickable consumable = item.GetComponent<ConsumablePickable>();
                if (consumable) {
                    Debug.Log("Consume item. Call Rpc");
                    consumable.DoConsume_RPC();
                }
            }

        }
    }

}
