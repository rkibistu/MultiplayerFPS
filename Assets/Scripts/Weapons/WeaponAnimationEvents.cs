using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponEvent {
    None = 0,
    Detache,
    Throw,
    Reload,
    Reatache,
}


public class WeaponAnimationEvents : MonoBehaviour {

    [SerializeField]
    private Vector3 ThrowDirection = new Vector3(2, 3, 0);
    [SerializeField]
    private bool HasCollider = true;
    [SerializeField]
    [Tooltip("hand used to reload. Only if the weapon has a magazine to change")]
    private Transform _leftHand;

    [Header("Sound")]
    [SerializeField]
    private Transform _reloadAudioEffectsRoot;
    [SerializeField]
    private AudioSetup _detacheAudioSetup;
    [SerializeField]
    private AudioSetup _throwAudioSetup;
    [SerializeField]
    private AudioSetup _reloadAudioSetup;
    [SerializeField]
    private AudioSetup _reatacheAudioSetup;

    private AudioEffect[] _reloadAudioEffects;


    //the magazine object seen in the hand
    private GameObject _magazineHand;
    // the maagzine object seen beeing thrown
    private GameObject _magazineThrown;
    private Rigidbody _magazineThrownRB;
    // the weapon
    private Weapon _currentWeapon;

    // direction where we throw the magazin
    private Vector3 _direction;
    // agent velocity when start throw
    private Vector3 _referenceVelocity;
    


    private void Awake() {

        _currentWeapon = GetComponent<Weapon>();
        if(_reloadAudioEffectsRoot != null) {

            _reloadAudioEffects = _reloadAudioEffectsRoot.GetComponentsInChildren<AudioEffect>();
        }

        
    }

    private void Start() {
        Debug.Log("Start blaster");
        if (_currentWeapon.Magazine) {
            Debug.Log("has amgazine -> settings");

            _magazineHand = Instantiate(_currentWeapon.Magazine, _leftHand, true);
            _magazineHand.SetActive(false);

            _magazineThrown = Instantiate(_magazineHand, _magazineHand.transform.position, _magazineHand.transform.rotation);
            _magazineThrownRB = _magazineThrown.AddComponent<Rigidbody>();
            if (HasCollider)
                _magazineThrown.AddComponent<BoxCollider>();
            _magazineThrown.SetActive(false);
        }
    }

    public void OnWeaponAnimationEvent(WeaponEvent weaponEvent) {
        TriggerWeaponEvent(weaponEvent);
    }
    public void PlaySound(WeaponEvent weaponEvent) {
        
        if (weaponEvent == WeaponEvent.None)
            return;

        switch (weaponEvent) {
            case WeaponEvent.Detache:
                _reloadAudioEffects.PlaySound(_detacheAudioSetup, EForceBehaviour.ForceAny);
                break;
            case WeaponEvent.Throw:
                _reloadAudioEffects.PlaySound(_throwAudioSetup, EForceBehaviour.ForceAny);
                break;
            case WeaponEvent.Reload:
                _reloadAudioEffects.PlaySound(_reloadAudioSetup, EForceBehaviour.ForceAny);
                break;
            case WeaponEvent.Reatache:
                _reloadAudioEffects.PlaySound(_reatacheAudioSetup, EForceBehaviour.ForceAny);
                break;
        }
    }
    public void StopAllSounds() {

        Debug.LogWarning("Stop all sounds! Do i neet it?");
    }

    private void TriggerWeaponEvent(WeaponEvent weaponEvent) {
        switch (weaponEvent) {

            case WeaponEvent.Detache:
                DetacheMagazine();
                break;

            case WeaponEvent.Throw:
                ThrowMagazine();
                break;

            case WeaponEvent.Reload:
                ReloadMagazine();
                break;

            case WeaponEvent.Reatache:
                ReatacheMagazine();
                break;
        }
    }

    private void DetacheMagazine() {

        MoveTo(_magazineHand.transform, _currentWeapon.Magazine.transform);
        _magazineHand.SetActive(true);
        _currentWeapon.Magazine.SetActive(false);
    }
    private void ThrowMagazine() {

        MoveTo(_magazineThrown.transform, _magazineHand.transform);
        _magazineThrown.SetActive(true);

        // Atentie aici! axele sunt puse aiurea din cauza la rotatia obiectului (up, forward, right)
        _direction = -transform.up * ThrowDirection.x - transform.right * ThrowDirection.z - transform.forward * ThrowDirection.y;
        _referenceVelocity = RoomPlayer.LocalRoomPlayer.ActiveAgent.KCC.FixedData.DesiredVelocity;
        _magazineThrownRB.velocity = Vector3.zero;
        _magazineThrownRB.AddForce(_direction + _referenceVelocity, ForceMode.Impulse);


        _magazineHand.SetActive(false);
    }
    private void ReloadMagazine() {

        _magazineHand.SetActive(true);
    }
    private void ReatacheMagazine() {

        _currentWeapon.Magazine.SetActive(true);
        _magazineHand.SetActive(false);
    }

    private void MoveTo(Transform toMove, Transform dest) {

        toMove.position = dest.position;
        toMove.rotation = dest.rotation;
    }
}
