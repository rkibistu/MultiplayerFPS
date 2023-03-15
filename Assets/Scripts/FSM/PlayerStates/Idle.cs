using Fusion.KCC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[Serializable]
public class Idle : Jumpable {

    private string _stateName = "[Idle]";


    private float _threshHold = 0.1f;

    public Idle(AgentStateMachine stateMachine) : base("Idle", stateMachine) {

    }
    public override void ProcessEarlyFixedInput() {
        base.ProcessEarlyFixedInput();
        //Verifica input sa vedem daca trebuie schimbata starea curenta

       // Debug.Log($"[{_agentStateMachine.Id}]Owner from Idle: " + (_agentStateMachine.Owner == null).ToString());

        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;
       
        var input = _agentStateMachine.Owner.Input.FixedInput;


      

        // Calculate input direction based on recently updated look rotation (the change propagates internally also to KCCData.TransformRotation)
        Vector3 inputDirection = _agentStateMachine.KCC.FixedData.TransformRotation * new Vector3(input.MoveDirection.x, 0.0f, input.MoveDirection.y);

        // datorita AirKCCProcessor (detecteza automat cand e in aer) -> in aer, schimabrile de directie for vi mai greoaie.
        _agentStateMachine.KCC.SetInputDirection(inputDirection);

        if (Mathf.Abs(inputDirection.x) >= _threshHold || Mathf.Abs(inputDirection.y) >= _threshHold || Mathf.Abs(inputDirection.z) >= _threshHold) {

            //change to run state
            //Debug.Log("Change state to Walk: " + (_agentStateMachine.Owner == null).ToString());
            _agentStateMachine.ChangeState(EPlayerStates.Walk);
            return;
        }

        // Actualizeza valorile din Animator penmtru animatia de Run
        _agentStateMachine.Animator.SetRunBlendTree(_agentStateMachine.KCC.FixedData.RealVelocity, 8f, _agentStateMachine.transform);

    }

    public override void OnEarlyFixedUpdate() {
        base.OnEarlyFixedUpdate();

    }

    public override void OnFixedUpdate() {
        base.OnFixedUpdate();

        // Regular fixed update for Agent class.
        // Executed after all agent KCC updates and before HitboxManager.
    }
   
    public override void ProcessLateFixedInput() {
        base.ProcessLateFixedInput();
        // Executed after HitboxManager. Process other non-movement actions like shooting.
        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;
    }

    public override void OnLateFixedUpdate() {
        base.OnLateFixedUpdate();

    }

    public override void ProcessRenderInput() {
        base.ProcessRenderInput();

        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;

    }

    public override void OnEarlyRender() {
        base.OnEarlyRender();


    }

    public override void OnRender() {
        base.OnRender();

    }

    public override void OnLateRender() {
        base.OnLateRender();
        // Setting base camera transform based on handle


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

        // play RunBlendTree state from animator.
        _agentStateMachine.Animator.Play(_agentStateMachine.Animator.RunBlendTree);

    }
    public override void Exit() {
        base.Exit();


    }
}
