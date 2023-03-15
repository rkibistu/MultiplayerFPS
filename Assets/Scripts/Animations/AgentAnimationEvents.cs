using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimationEvents : MonoBehaviour
{
    [SerializeField]
    private Animator _rootAnimator;

    [Header("Sound")]
    [SerializeField]
    private Transform _stepAudioEffectRoot;
    [SerializeField]
    private AudioSetup _stepSound;
    [SerializeField]
    private AudioSetup _jumpSetup;
    [SerializeField]
    private AudioSetup _landSetup;
    private AudioEffect[] _audioEffects;

    private void Awake() {

        if (_stepAudioEffectRoot != null) {
            _audioEffects = _stepAudioEffectRoot.GetComponentsInChildren<AudioEffect>(true);
        }
    }


    //Called by aniamtion events: during deathFlyingBack animation
    //      scope: make the body touch the ground (dieing animation doesn t move visual body down)
    private void PlayDeathHelperAnimation() {

        _rootAnimator.Play("DeathFlyingBack_Helper");
    }

    //Called by animatin events during run aniamtions (all directions)
    private void PlayStepSound() {

        _audioEffects.PlaySound(_stepSound, EForceBehaviour.ForceAny);
    }

    //Called by animatin events when play jump aniamtion
    private void PlayJumpSound() {

        _audioEffects.PlaySound(_jumpSetup, EForceBehaviour.ForceAny);
    }

    //Called by animatin events when play jump aniamtion
    private void PlayLandSound() {

        _audioEffects.PlaySound(_landSetup, EForceBehaviour.ForceAny);
    }

}
