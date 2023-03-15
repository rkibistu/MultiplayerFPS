using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkEvents;


// clasa asta reprezinta practic un Canvas specific petru gameplay

public class UIGameplayView : UIView {
    // PRIVATE MEMBERS
    [SerializeField]
    private GameObject _observedAgentRoot;
    [SerializeField]
    private List<GameObject> _disableWhenDead;

    private UIHealth _health;
    private UICrosshair _crosshair;
    private UIHitNumbers _hitNumbers;
    private UIScreenEffects _screenEffects;
    private UIAmmo _ammo;
    private UIRoundTimer _roundTimer;
    private UIScope _uiScope;

    private AgentStateMachine _observedAgent;
    private Gameplay _runningGameplay;

    private bool _aliveUI = true;

    // UIView INTERFACE

    //seteaza agentul pe care se centreaza focusul informatiei
    // de la ce agent ia viata, ammo, etc...
    protected override void OnInitialize() {

        base.OnInitialize();

        ClearObservedAgent(true);

        AgentStateMachine agent = Context.Instance.Runner.GetPlayerObject(Context.Instance.Runner.LocalPlayer).GetComponent<RoomPlayer>().ActiveAgent;
        SetObservedAgent(agent);

        _runningGameplay = Context.Instance.Gameplay;

        _health = GetComponentInChildren<UIHealth>(true);
        _crosshair = GetComponentInChildren<UICrosshair>(true);
        _hitNumbers = GetComponentInChildren<UIHitNumbers>(true);
        _screenEffects = GetComponentInChildren<UIScreenEffects>(true);
        _ammo = GetComponentInChildren<UIAmmo>(true);
        _roundTimer = GetComponentInChildren<UIRoundTimer>(true);
        _uiScope = GetComponentInChildren<UIScope>(true);
    }

    // actualizeaza informatia -> health, ammo, etc.
    protected override void OnTick() {
        base.OnTick();


        if (Context.Instance.Runner.IsRunning == false)
            return;

        if (Context.Instance.Gameplay == null)
            return;

        if (_observedAgent == null)
            return;

        

        _health.UpdateHealth(_observedAgent.Health);
        _ammo.UpdateAmmo(_observedAgent.Weapons.CurrentWeapon.WeaponMagazine);
        _ammo.UpdateWeaponImage(_observedAgent.Weapons.CurrentWeapon);
        _screenEffects.UpdateEffects(_observedAgent);
        _roundTimer.UpdateTimerValue(_runningGameplay.Timer);
        _uiScope.UpdateScope(_observedAgent.Weapons.CurrentWeapon.WeaponScope);

        SetAliveUI();
    }

    // PRIVATE METHODS

    // sterge agentul care e in focus si dezactiveza canvasul daca vrei
    private void ClearObservedAgent(bool hideElements) {
        if (_observedAgent != null) {
            _observedAgent.Health.HitPerformed -= OnHitPerformed;
            _observedAgent.Health.HitTaken -= OnHitTaken;
            _observedAgent.AgentDespawned -= OnAgentDespawned;

            _observedAgent = null;
        }

        if (hideElements == true) {
            _observedAgentRoot.SetActive(false);
        }
    }

    // seteaza agentul care sa fie in focus
    // seteaza event-uri pentru modificare de viata sau despawn-ul agentului
    private void SetObservedAgent(AgentStateMachine agent, bool force = false) {

        if (agent == _observedAgent && force == false)
            return;

        ClearObservedAgent(false);
        _observedAgent = agent;

        if (agent != null) {
            agent.Health.HitPerformed += OnHitPerformed;
            agent.Health.HitTaken += OnHitTaken;
            agent.AgentDespawned += OnAgentDespawned;
        }

        _observedAgentRoot.SetActive(true);
    }

    // apeleaza cand se confirma ca unul din hiturile agentului a lovit cu succes
    private void OnHitPerformed(HitData hitData) {

        _crosshair.HitPerformed(hitData);
        _hitNumbers.HitPerformed(hitData);
    }

    //apeleaza cand agentul primeste un hit
    private void OnHitTaken(HitData hitData) {

        _screenEffects.OnHitTaken(hitData);
    }

    // apeleaza cand agentul de despawneaza
    private void OnAgentDespawned(AgentStateMachine agent) {
        ClearObservedAgent(false);
    }


    //disable every game object that dowsn't need to be enabled when we die
    private void SetAliveUI() {

        if(_observedAgent.Health.CurrentHealth <= 0) {

            foreach (var ui in _disableWhenDead) {

                ui.SetActive(false);
            }
            _aliveUI = false;
        }
        if(!_aliveUI && (_observedAgent.Health.CurrentHealth > 0)) {

            foreach (var ui in _disableWhenDead) {

                ui.SetActive(true);
            }
        }
        

        
    }
}
