using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ShootingState : PlayerMainState
{
    public ShootingState(string name, AgentStateMachine stateMachine) : base(name, stateMachine) {

    }

    public override void ProcessLateFixedInput() {
        base.ProcessLateFixedInput();
        // Executed after HitboxManager. Process other non-movement actions like shooting.
        if (_agentStateMachine.Owner == null || _agentStateMachine.Health.IsAlive == false)
            return;

        if (_agentStateMachine.Weapons != null)
            _agentStateMachine.Weapons.ProcessInput(_agentStateMachine.Owner.Input);
    }
}
