using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ConsumablePickable : PickableItem {
    [SerializeField]
    [Tooltip("Choose one of the public functions this class expose")]
    private UnityEvent<PlayerRef> Consume;

    [SerializeField]
    [Tooltip("The amount of cantity to consume")]
    private float _amount;

    //Called by input authority agent when pickup a consumable
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void DoConsume_RPC(RpcInfo info = default) {

        if (Consume == null) {
            Debug.LogError("This should not be null. It is a consumable. Should do soemthign");
            return;
        }
        Debug.Log("Consume!");

        Consume.Invoke(info.Source);
        SelfDestroy();
    }

    //Method called by consumables
    // It is bind in editor using unityEvents
    public void HealAgent(PlayerRef playerRef) {

        AgentStateMachine agent;
        if (playerRef.IsNone) {
            // playerRef is none when a RPC is called by the host
            agent = RoomPlayer.LocalRoomPlayer.ActiveAgent;
        }
        else {
            agent = Context.Instance.Gameplay.Players[playerRef].ActiveAgent;
        }

        agent.Health.AddHealth(_amount);
    }

    public void AddAmmoToAgent() {

        Debug.LogWarning("Not implemented yet");
    }

}
