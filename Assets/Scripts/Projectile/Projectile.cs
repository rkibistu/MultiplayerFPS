using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{

    // PUBLIC MEMBERS
    public byte PrefabId { get; private set; }
    public bool IsFinished { get; protected set; }
    public bool IsDiscarded { get; private set; }


    public virtual bool NeedsInterpolationData => false;


    // PRIVATE MEMBERS

    [SerializeField]
    private GameObject _impactEffectPrefab;
    [SerializeField]
    private float _impactEffectReturnTime = 2f;
    [SerializeField, Tooltip("Standalone effect that will be spawned through NetworkRunner")]
    private NetworkObject _impactObjectPrefab;

    private TrailRenderer[] _trails;



    // PUBLIC METHODS
    public abstract ProjectileData GetFireData(NetworkRunner runner, Vector3 firePosition, Vector3 fireDirection);
    public abstract void OnFixedUpdate(ProjectileContext context, ref ProjectileData data);


    public void Activate(ProjectileContext context, ref ProjectileData data) {
        PrefabId = data.PrefabId;
        IsFinished = false;
        IsDiscarded = false;

        OnActivated(context, ref data);
    }

    public void Deactivate(ProjectileContext context) {
        IsFinished = true;

        OnDeactivated(context);
    }

    public void Discard() {
        IsDiscarded = true;
        OnDiscarded();
    }


    public virtual void OnRender(ProjectileContext context, ref ProjectileData data) {
    }


    // MONOBEHAVIOR
    protected virtual void Awake() {
        _trails = GetComponentsInChildren<TrailRenderer>(true);
    }

    // PROTECTED METHODS

    protected virtual void OnActivated(ProjectileContext context, ref ProjectileData data) {
    }

    protected virtual void OnDeactivated(ProjectileContext context) {
        // Clear trials before returning to cache
        for (int i = 0; i < _trails.Length; i++) {
            _trails[i].Clear();
        }
    }
    protected virtual void OnDiscarded() {
        IsFinished = true;
    }

    protected void SpawnImpactVisual(ProjectileContext context, ref ProjectileData data) {
        if (context.Runner.Stage != default) {
            Debug.LogError("Call SpawnImpactVisual only from render method");
            return;
        }

        if (data.ImpactPosition == Vector3.zero)
            return;

        if (_impactEffectPrefab != null) {

            var impact = context.Cache.Get(_impactEffectPrefab);
            impact.SetActive(true);


            impact.transform.SetPositionAndRotation(data.ImpactPosition, Quaternion.identity);
            context.Runner.MoveToRunnerScene(impact);

            context.Cache.ReturnDeferred(impact, _impactEffectReturnTime);
        }
    }

    protected void SpawnImpact(ProjectileContext context, ref ProjectileData data, Vector3 position, Vector3 normal) {
        if (context.Runner.Stage == default) {
            Debug.LogError("Call SpawnImpact only from fixed update");
            return;
        }

        if (position == Vector3.zero)
            return;

        data.ImpactPosition = position;
        //data.ImpactNormal = normal;

        if (_impactObjectPrefab != null) {
            var key = new NetworkObjectPredictionKey() { AsInt = context.Runner.Tick * (context.InputAuthority + _impactObjectPrefab.NetworkGuid.GetHashCode()) };
            context.Runner.Spawn(_impactObjectPrefab, position, Quaternion.LookRotation(normal), context.InputAuthority, null, key);
        }
    }

}
