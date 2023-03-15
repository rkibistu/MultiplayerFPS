using Fusion.KCC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.UI.GridLayoutGroup;

[Serializable]
public class Walk : Jumpable {
    private string _stateName = "[Walk]";
    public Walk(AgentStateMachine stateMachine) : base("Walk", stateMachine) {

    }
    public override void ProcessEarlyFixedInput() {
        base.ProcessEarlyFixedInput();
        // Verifica daca schimbam starea +
        // Seteaza directia de deplasare pentru KCC (KCC va face msicarile efective)
        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;

        var input = _agentStateMachine.Owner.Input;
        // Calculate input direction based on recently updated look rotation (the change propagates internally also to KCCData.TransformRotation)
        Vector3 inputDirection = _agentStateMachine.KCC.FixedData.TransformRotation * new Vector3(input.FixedInput.MoveDirection.x, 0.0f, input.FixedInput.MoveDirection.y);
       
        //Check to change state
        if(inputDirection.IsAlmostZero()) {

            //daca nu se apasa niciun buton pentru miscare -> go to idle
            _agentStateMachine.ChangeState(EPlayerStates.Idle);
            
        }

        if(_agentStateMachine.KCC.FixedData.IsGrounded == true) {

            _agentStateMachine.KCC.SetSprint(input.IsSet(EInputButtons.Sprint));
        }

        //Seteaza directia inputul, astfel incat KCC sa aplice miscarea
        _agentStateMachine.KCC.SetInputDirection(inputDirection);

        // Actualizeza valorile din Animator penmtru animatia de Run
        _agentStateMachine.Animator.SetRunBlendTree(_agentStateMachine.KCC.FixedData.RealVelocity, 8f, _agentStateMachine.transform);
    }
    public override void OnEarlyFixedUpdate() {
        base.OnEarlyFixedUpdate();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;

    }
    public override void OnFixedUpdate() {
        base.OnFixedUpdate();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
    }

    public override void ProcessLateFixedInput() {
        base.ProcessLateFixedInput();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;

    }
    public override void OnLateFixedUpdate() {
        base.OnLateFixedUpdate();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
    }

    public override void ProcessRenderInput() {
        base.ProcessRenderInput();
        // Here we process input and set properties related to movement
        // For following lines, we should use Input.RenderInput and Input.CachedInput only. These properties hold input for render updates.
        // Input.RenderInput holds input for current render frame.
        // Input.CachedInput holds combined input for all render frames from last fixed update. This property will be used to set input for next fixed update (polled by Fusion).
        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;

        var input = _agentStateMachine.Owner.Input;
        Vector3 inputDirection = default;

        // MoveDirection values from previous render frames are already consumed and applied by KCC, so we use Input.RenderInput (non-accumulated input for this frame)
        Vector3 moveDirection = input.RenderInput.MoveDirection.X0Y();
        if (moveDirection.IsZero() == false) {
            inputDirection = _agentStateMachine.KCC.RenderData.TransformRotation * moveDirection;
        }

        _agentStateMachine.KCC.SetInputDirection(inputDirection);

        if(_agentStateMachine.KCC.RenderData.IsGrounded == true) {

            _agentStateMachine.KCC.SetSprint(input.IsSet(EInputButtons.Sprint));
        }
    }

    public override void OnEarlyRender() {
        base.OnEarlyRender();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;

    }

    public override void OnRender() {
        base.OnRender();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
    }

    public override void OnLateRender() {
        base.OnLateRender();

        //if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
        //    return;
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
