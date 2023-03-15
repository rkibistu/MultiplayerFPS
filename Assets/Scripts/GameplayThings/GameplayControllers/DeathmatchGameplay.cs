using Fusion;
using JetBrains.Annotations;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Extinde clasa Gameplay (vezi gameplay)
/// Setari specifice gameplayului de tip Deathmatch
/// 
/// 
/// Seteaza ca atunci cand un jucator moare, sa fie automat respawnat cu un mic delay.
/// 
/// </summary>

public class DeathmatchGameplay : Gameplay {

    // PUBLIC MEMBERS

    public float _reviveDelay = 3f;

    // PRIVATE MEMBERS
    private bool _isReviveExecuting = false;


    // GameplayController INTERFACE
    protected override void OnSpawned() {
        base.OnSpawned();

        Debug.Log("Sunt initial: " + Players.Count + " players");

        _timer._onTiemrExpired += OnRoundEnd;
        _timer.StartTimer(Runner, _roundDuration);
    }
    protected override void OnFatalHitTaken(HitData hitData) {
        base.OnFatalHitTaken(hitData);

        GameObject agent = hitData.Target.GameObject;

        StartCoroutine(RevivePlayerWithDelay(agent, _reviveDelay));
    }

    protected override void OnPlayerAgentSpawned(AgentStateMachine agent) {
        base.OnPlayerAgentSpawned(agent);

        SetPositionToSpawnPoint(agent);
    }

    // PRIVATE METHODS

    // seteaza viata jucatorului la valoarea maxima dupa un delay de timp in secunde
    private IEnumerator RevivePlayerWithDelay(GameObject playerAgent, float delay) {

        if (_isReviveExecuting)
            yield break;

        _isReviveExecuting = true;
        yield return new WaitForSeconds(delay);

        var health = playerAgent.GetComponent<Health>();
        health.ResetHealth();


       
        SetPositionToSpawnPoint(playerAgent.GetComponent<AgentStateMachine>());
        _isReviveExecuting = false;
    }

    // Alege random din unul de punctele de spawn si muta playerul acolo
    private void SetPositionToSpawnPoint(AgentStateMachine agent) {

        Transform spawnPoint = RandomSpawnPoint();
        Debug.Log("Move agent to: " + spawnPoint.position);
        agent.MoveTo(spawnPoint.position);
    }

    private void OnRoundEnd() {

        Debug.Log("Round ended, timer expired");
        RoomPlayer.LocalRoomPlayer.RoundEnd(_activateUI);
    }

  
}
