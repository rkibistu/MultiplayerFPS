using Fusion.KCC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[Serializable]
public class Jump : ShootingState
{
    // PRIVATE MEMBERS
    [SerializeField]
    private Vector3 _jumpImpulse = new Vector3(0f, 6f, 0f);

    private bool _groundDetachment = false;
    

    // PUBLIC METHODS
    public Jump(AgentStateMachine agentStateMachine) : base("Jump", agentStateMachine) {

    }


    //BASESTATE INTERFACE
    public override void ProcessEarlyFixedInput() {
        base.ProcessEarlyFixedInput();

        // apelata automat si la intrarea in stare
        
        //marcheaza cand a inceput jump-ul efectiv (s-a desprins de sol)
        if (!_agentStateMachine.KCC.FixedData.IsGrounded && !_groundDetachment)
            _groundDetachment = true;

        //check if grounded -> change to idle
        //do the check only after the jump has started (se desprinde de sol)
        //        othewise the Jump state is gonna finish without executing
        if (_agentStateMachine.KCC.FixedData.IsGrounded && _groundDetachment) {

            
            _agentStateMachine.ChangeState(EPlayerStates.Idle);
        }

        DoPhysicsJump();

    }

    public override void ProcessRenderInput() {
        base.ProcessRenderInput();
        // Here we process input and set properties related to movement / look.
        // For following lines, we should use Input.RenderInput and Input.CachedInput only. These properties hold input for render updates.
        // Input.RenderInput holds input for current render frame.
        // Input.CachedInput holds combined input for all render frames from last fixed update. This property will be used to set input for next fixed update (polled by Fusion).

        DoRenderJump();
    }


    // PRIVATE METHODS
    private void DoPhysicsJump() {

        // Here we apply the jump physics. This method is called only when Jump button is pressed;
        // For following lines, we should use Input.FixedInput only. This property holds input for fixed updates.
        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;

        var input = _agentStateMachine.Owner.Input.FixedInput;

        // Calculate input direction based on recently updated look rotation (the change propagates internally also to KCCData.TransformRotation)
        Vector3 inputDirection = _agentStateMachine.KCC.FixedData.TransformRotation * new Vector3(input.MoveDirection.x, 0.0f, input.MoveDirection.y);
        
        // datorita AirKCCProcessor (detecteza automat cand e in aer) -> in aer, schimabrile de directie for vi mai greoaie.
        _agentStateMachine.KCC.SetInputDirection(inputDirection);

        //Verificare suplimentara ca butonul de jump a fost apasat
        if (_agentStateMachine.Owner.Input.WasPressed(EInputButtons.Jump) == true) {
            // By default the character jumps forward in facing direction
            Quaternion jumpRotation = _agentStateMachine.KCC.FixedData.TransformRotation;

            if (inputDirection.IsAlmostZero() == false) {
                // If we are moving, jump in that direction instead
                jumpRotation = Quaternion.LookRotation(inputDirection);
            }

            // Applying jump impulse
            _agentStateMachine.KCC.Jump(jumpRotation * _jumpImpulse);
        }

       
    }
    private void DoRenderJump() {
        // Here we process input and set properties related to jump movement
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

        // Jump is extrapolated for render as well.
        if (_agentStateMachine.Owner.Input.WasPressed(EInputButtons.Jump) == true) {
            // By default the character jumps forward in facing direction
            Quaternion jumpRotation = _agentStateMachine.KCC.RenderData.TransformRotation;

            if (inputDirection.IsZero() == false) {
                // If we are moving, jump in that direction instead
                jumpRotation = Quaternion.LookRotation(inputDirection);
            }

            _agentStateMachine.KCC.Jump(jumpRotation * _jumpImpulse);
        }
    }

    public override void Enter() {
        base.Enter();

        _agentStateMachine.Animator.PlayJumpUp();
        _groundDetachment = false;
    }
    public override void Exit() {
        base.Exit();

        _agentStateMachine.Animator.PlayJumpDown();
        _groundDetachment = false;
    }
}
