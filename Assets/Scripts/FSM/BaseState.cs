using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseState
{
    public string _name;

    protected AgentStateMachine _agentStateMachine;


    public BaseState(string name, AgentStateMachine stateMachine) {
        this._name = name;
        this._agentStateMachine = stateMachine;
    }

    public virtual void StateStart() { }
    public virtual void Enter() { }

    public abstract void ProcessEarlyFixedInput();
    public abstract void ProcessLateFixedInput();
    public abstract void ProcessRenderInput();

    public virtual void OnSpawned() { }
    public virtual void OnDespawned() { }
    public virtual void OnEarlyFixedUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateFixedUpdate() { }
    public virtual void OnEarlyRender() { }
    public virtual void OnRender() { }
    public virtual void OnLateRender() { }

    public virtual void Exit() { }
    public virtual void DrawGizmos() { }
}
