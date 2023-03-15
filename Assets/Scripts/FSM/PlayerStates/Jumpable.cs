using Fusion.KCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class Jumpable : ShootingState
{

    public Jumpable(string name, AgentStateMachine stateMachine) : base(name, stateMachine) {

    }

    public override void ProcessEarlyFixedInput() {
        base.ProcessEarlyFixedInput();

        // Here we process input related to jump movement. If jump button pressed -> change state to Jump state
        // For following lines, we should use Input.FixedInput only. This property holds input for fixed updates.

        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;

        var input = _agentStateMachine.Owner.Input.FixedInput;
        Vector3 inputDirection = _agentStateMachine.KCC.FixedData.TransformRotation * new Vector3(input.MoveDirection.x, 0.0f, input.MoveDirection.y);
        if (_agentStateMachine.Owner.Input.WasPressed(EInputButtons.Jump) == true) {

            _agentStateMachine.ChangeState(EPlayerStates.Jump);
        }
    }
}
