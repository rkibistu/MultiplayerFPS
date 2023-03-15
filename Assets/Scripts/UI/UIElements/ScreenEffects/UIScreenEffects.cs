using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkEvents;
using UnityEngine.EventSystems;
using UnityEngineInternal;

public class UIScreenEffects : UIWidget {
    // PRIVATE METHODS

    [SerializeField]
    private CanvasGroup _hitGroup;
    [SerializeField]
    private CanvasGroup _bloodScreen1;
    [SerializeField]
    [Tooltip("a value in range 0-1, the screen is seen when health is lower")]
    [Range(0f, 1f)]
    private float _threshhold1 = 0.5f;
    [SerializeField]
    private CanvasGroup _bloodScreen2;
    [SerializeField]
    [Tooltip("a value in range 0-1, the screen is seen when health is lower")]
    [Range(0f, 1f)]
    private float _threshhold2 = 0.25f;
    [SerializeField]
    private UIBehaviour _deathGroup;
    [SerializeField]
    private GameObject _breakableScreen;

    [Header("Animation")]
    [SerializeField]
    private float _hitFadeInDuratio = 0.1f;
    [SerializeField]
    private float _hitFadeOutDuration = 0.7f;

    [Header("Audio")]
    [SerializeField]
    private AudioSetup _hitSound;
    [SerializeField]
    private AudioSetup _deathSound;

    private AgentStateMachine _agent;

    // PUBLIC METHODS

    public void OnHitTaken(HitData hit) {
        if (hit.Amount <= 0)
            return;

        if (hit.Action == EHitAction.Damage) {


            ShowHitEffects(hit);

            if (hit.IsFatal == true) {
                _deathGroup.SetActive(true);
                PlaySound(_deathSound, EForceBehaviour.ForceAny);
            }
        }
    }

    public void UpdateEffects(AgentStateMachine agent) {
        if (!_agent)
            _agent = agent;

        
        _deathGroup.SetActive(agent.Health.IsAlive == false);
        _breakableScreen.SetActive(agent.Health.IsAlive == false);
    }

    // MONOBEHAVIOUR

    protected override void OnVisible() {
        base.OnVisible();

        _hitGroup.SetActive(true);
        _hitGroup.alpha = 0f;

        _bloodScreen1.SetActive(true);
        _bloodScreen1.alpha = 0f;

        _bloodScreen2.SetActive(true);
        _bloodScreen2.alpha = 0f;

        _deathGroup.SetActive(false);
    }

    // PRIVATE METHODS

    private void ShowHit(CanvasGroup group, float targetAlpha) {
        DOTween.Kill(group);

        group.DOFade(targetAlpha, _hitFadeInDuratio);
        group.DOFade(0f, _hitFadeOutDuration).SetDelay(_hitFadeInDuratio);
    }

    private void ShowHitEffects(HitData hit) {
        float alpha = Mathf.Lerp(0, 1f, hit.Amount / 20f);

        ShowHit(_hitGroup, alpha);

        if (_agent) {

            if (_bloodScreen1 && (_agent.Health.CurrentHealth < _agent.Health.MaxHealth * _threshhold1))
                ShowHit(_bloodScreen1, alpha);
            if (_bloodScreen2 && (_agent.Health.CurrentHealth < _agent.Health.MaxHealth * _threshhold2))
                ShowHit(_bloodScreen2, alpha);
        }
        
        PlaySound(_hitSound, EForceBehaviour.ForceAny);
    }
}

