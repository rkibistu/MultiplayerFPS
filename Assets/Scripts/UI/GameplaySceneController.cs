using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySceneController : SceneController
{
    // PRIVATE MEMBERS

    private KillAnnouncer _killAnnouncer;

    // Scene INTERFACE

    protected override void OnActivate() {
       
        base.OnActivate();

        _killAnnouncer = GetComponentInChildren<KillAnnouncer>(true);
        Debug.LogWarning(_killAnnouncer == null);
    }

    // PUBLIC METHODS

    //called by Gameplay (gameplay controller) when someone dies
    // hitData aprameter cotnains all info about the death (instigator,target,etc)
    public void AnnounceKill(HitData hitData, Sprite middleIcon) {

        RoomPlayer instigator = Context.Instance.Gameplay.Players[hitData.InstigatorRef];

        AgentStateMachine targetAgent = hitData.Target.GameObject.GetComponent<AgentStateMachine>();
        RoomPlayer target = targetAgent.Owner;

        Debug.Log(instigator.Username + " killed " + target.Username);
        _killAnnouncer.CreateKillAnnouncement(instigator.Username.ToString(), target.Username.ToString(), instigator.ActiveAgent.Weapons.CurrentWeapon.Icon);
    }

}

