using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public abstract class PickableItem : NetworkBehaviour {
    [SerializeField]
    [Tooltip("The 3d model to be seen on the ground")]
    private GameObject _modelPrefab;
    [SerializeField]
    private GameObject _particleEffect;

    [Range(0f, 1f)]
    [Tooltip("Chanhe to be spawn by a loot box")]
    public float SpawnChance = 1f;


    [Header("Sound")]
    [SerializeField]
    private Transform _fireAudioEffectsRoot;
    [SerializeField]
    private AudioSetup _fireSound;
    private AudioEffect[] _fireAudioEffects;
    private float _despawnDelay = 0.5f;

    private GameObject _modelObject;
    private NetworkObject _networkObject;
    private BoxCollider _boxCollider;

    private void Awake() {

        if (_fireAudioEffectsRoot != null) {
            _fireAudioEffects = _fireAudioEffectsRoot.GetComponentsInChildren<AudioEffect>(true);
        }
    }

    private void OnEnable() {

        if (_modelObject == null) {
            _modelObject = Instantiate(_modelPrefab, transform.position, _modelPrefab.transform.rotation, transform);
        }
        if (_networkObject == null) {
            _networkObject = GetComponent<NetworkObject>();
        }
        if (_boxCollider == null) {
            _boxCollider = GetComponent<BoxCollider>();
        }
        if (_particleEffect) {
            _particleEffect.SetActive(true);
        }

        ActivateCollider();
    }

    //We have to despawn the object after a delay so the sound to have time to be played
    public void SelfDestroy() {

        DezactivateCollider();
        DestroyModel_RPC();
        _fireAudioEffects.PlaySound(_fireSound, EForceBehaviour.ForceAny);
        Invoke("DespawnAfterDelay", _despawnDelay); //despawn the object after a delay
    }

    //Only StateAuthority can call Despawn -> we need to call rpc
    private void DespawnAfterDelay() {

        Despawn_RPC();
    }
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void Despawn_RPC() {

        Context.Instance.Runner.Despawn(_networkObject);
    }


    // We despawn the object with a delay. But we want to destroy the model for all players as fast as we can.
    // So we call this method in SelfDestroy without delay
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void DestroyModel_RPC() {

        if (_particleEffect)
            _particleEffect.SetActive(false);
        if (_modelObject)
            Destroy(_modelObject);

    }


    private void DezactivateCollider() {

        _boxCollider.enabled = false;
    }
    private void ActivateCollider() {

        _boxCollider.enabled = true;
    }

}
