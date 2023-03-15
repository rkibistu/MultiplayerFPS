using Fusion;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Explicit)]
public struct ProjectileData : INetworkStruct {

    public bool IsActive { get { return _state.IsBitSet(0); } set { _state.SetBit(0, value); } }

    public bool IsFinished { get { return _state.IsBitSet(1); } set { _state.SetBit(1, value); } }

    [FieldOffset(0)]
    private byte _state;

    [FieldOffset(1)]
    public byte PrefabId;
    [FieldOffset(2)]
    public byte WeaponAction;

    [FieldOffset(7)]
    public Vector3 FirePosition;
    [FieldOffset(19)]
    public Vector3 FireVelocity;
    [FieldOffset(31)]
    public Vector3 ImpactPosition;
    [FieldOffset(3)]
    public int FireTick;
    [FieldOffset(55)]
    public KinematicData Kinematic;
}

public struct KinematicData : INetworkStruct {
    public NetworkBool HasStopped;
    public Vector3 FinishedPosition;
    public int StartTick;
    public byte BounceCount;
}
public struct ProjectileInterpolationData {
    public ProjectileData From;
    public ProjectileData To;
    public float Alpha;
}

public class ProjectileContext {

    public NetworkRunner Runner;
    public ObjectCache Cache;
    public PlayerRef InputAuthority;
    public int OwnerObjectInstanceID;

    // Barrel transform represents position from which projectile visuals should fly out
    // (actual projectile fire calculations are usually done from different point, for example camera)
    public Transform BarrelTransform;

    public float FloatTick;
    public bool Interpolate;
    public ProjectileInterpolationData InterpolationData;
}


public interface IProjectileManager {
    public void AddProjectile(Projectile projectilePrefab, Vector3 firePosition, Vector3 direction, byte weaponAction = 0);
}

public class ProjectileManager : NetworkBehaviour, IProjectileManager {

    [SerializeField]
    private bool _fullProxyPrediction = false;
    [SerializeField]
    private Projectile[] _projectilePrefabs;

    [Networked, Capacity(96)]
    private NetworkArray<ProjectileData> _projectiles { get; }
    [Networked]
    private int _projectileCount { get; set; }

    private Projectile[] _visibleProjectiles = new Projectile[128];
    private int _visibleProjectileCount;

    private ProjectileContext _projectileContext;

    private RawInterpolator _projectilesInterpolator;

  

    public void AddProjectile(Projectile projectilePrefab, Vector3 firePosition, Vector3 direction, byte weaponAction = 0) {
        var fireData = projectilePrefab.GetFireData(Runner, firePosition, direction);
        AddProjectile(projectilePrefab, fireData, weaponAction);
    }


    public void AddProjectile(Projectile projectilePrefab, ProjectileData data, byte weaponAction = 0) {
        int prefabIndex = _projectilePrefabs.IndexOf(projectilePrefab);

        if (prefabIndex < 0) {
            Debug.LogError($"Projectile {projectilePrefab} not found. Add it in ProjectileManager prefab array.");
            return;
        }

        data.PrefabId = (byte)prefabIndex;
        data.FireTick = Runner.Tick;
        data.IsActive = true;
        data.WeaponAction = weaponAction;

        int projectileIndex = _projectileCount % _projectiles.Length;

        var previousData = _projectiles[projectileIndex];
        if (previousData.IsActive == true && previousData.IsFinished == false) {
            Debug.LogError("No space for another projectile - projectile buffer should be increased or projectile lives too long");
        }

        _projectiles.Set(projectileIndex, data);

        _projectileCount++;
    }

    public void OnSpawned() {
        _visibleProjectileCount = _projectileCount;

        _projectileContext = new ProjectileContext() {
            Runner = Runner,
            Cache = Context.Instance.ObjectCache,
            InputAuthority = Object.InputAuthority,
            OwnerObjectInstanceID = gameObject.GetInstanceID(),
        };

        _projectilesInterpolator = GetInterpolator(nameof(_projectiles));
    }

    public override void Despawned(NetworkRunner runner, bool hasState) {
        for (int i = 0; i < _visibleProjectiles.Length; i++) {
            var projectile = _visibleProjectiles[i];
            if (projectile != null) {
                DestroyProjectile(_projectileContext,projectile);
                _visibleProjectiles[i] = null;
            }
        }
    }
    public void OnFixedUpdate() {
        // Projectile calculations are processed only on input or state authority
        // unless full proxy prediction is turned on
        if (IsProxy == true)
            return;

        _projectileContext.FloatTick = Runner.Tick;


        for (int i = 0; i < _projectiles.Length; i++) {
            var projectileData = _projectiles[i];

            if (projectileData.IsActive == false)
                continue;
            if (projectileData.IsFinished == true)
                continue;

            var prefab = _projectilePrefabs[projectileData.PrefabId];
            prefab.OnFixedUpdate(_projectileContext, ref projectileData);

            _projectiles.Set(i, projectileData);
        }
    }

    public void OnRender(Transform[] weaponBarrelTransforms) {
        // Visuals are not spawned on dedicated server at all
        if (Runner.Mode == SimulationModes.Server)
            return;

        RenderProjectiles(weaponBarrelTransforms);
    }

    private void RenderProjectiles(Transform[] weaponBarrelTransforms) {

        _projectilesInterpolator.TryGetArray(_projectiles, out var fromProjectiles, out var toProjectiles, out float interpolationAlpha);

        var simulation = Runner.Simulation;
        bool interpolate = IsProxy == true && _fullProxyPrediction == false;


        if (interpolate == true) {
            // For proxies we move projectile in snapshot interpolated time
            _projectileContext.FloatTick = simulation.InterpFrom.Tick + (simulation.InterpTo.Tick - simulation.InterpFrom.Tick) * simulation.InterpAlpha;
        }
        else {
            _projectileContext.FloatTick = simulation.Tick + simulation.StateAlpha;
        }

        int barrelTransformCount = weaponBarrelTransforms.Length;
        int bufferLength = _projectiles.Length;


        // Our predicted projectiles were not confirmed by the server, let's discard them
        for (int i = _projectileCount; i < _visibleProjectileCount; i++) {
            var projectile = _visibleProjectiles[i % bufferLength];
            if (projectile != null) {
                // We are not destroying projectile immediately,
                // projectile can decide itself how to react
                projectile.Discard();
            }
        }

        int minFireTick = Runner.Tick - (int)(Runner.Simulation.Config.TickRate * 0.5f);


        for (int i = _visibleProjectileCount; i < _projectileCount; i++) {
            int index = i % bufferLength;
            var projectileData = _projectiles[index];

            // Projectile is long time finished, do not spawn visuals for it
            // Note: We cannot check just IsFinished, because some projectiles are finished
            // immediately in one tick but the visuals could be longer running
            if (projectileData.IsFinished == true && projectileData.FireTick < minFireTick)
                continue;

            if (_visibleProjectiles[index] != null) {
                Debug.LogError("No space for another projectile gameobject - projectile buffer should be increased or projectile lives too long");
                DestroyProjectile(_projectileContext,_visibleProjectiles[index]);
            }

            byte weaponAction = projectileData.WeaponAction;
            if (weaponAction >= barrelTransformCount) {
                Debug.LogError($"Create: Barrel transform with index {weaponAction} not present");
                weaponAction = 0;
            }

            _projectileContext.BarrelTransform = weaponBarrelTransforms[weaponAction];
            _visibleProjectiles[index] = CreateProjectile(_projectileContext, ref projectileData);
        }

        // Update all visible projectiles
        for (int i = 0; i < bufferLength; i++) {
            var projectile = _visibleProjectiles[i];
            if (projectile == null)
                continue;

            if (projectile.IsDiscarded == false) {
                var data = _projectiles[i];

                if (data.PrefabId != projectile.PrefabId) {
                    Debug.LogError($"{Runner.name}: Incorrect spawned prefab. Should be {data.PrefabId}, actual {projectile.PrefabId}. This should not happen.");
                    DestroyProjectile(_projectileContext,projectile);
                    _visibleProjectiles[i] = null;
                    continue;
                }

                bool interpolateProjectile = interpolate == true && projectile.NeedsInterpolationData;

                // Prepare interpolation data if needed
                ProjectileInterpolationData interpolationData = default;
                if (interpolateProjectile == true) {
                    interpolationData.From = fromProjectiles.Get(i);
                    interpolationData.To = toProjectiles.Get(i);
                    interpolationData.Alpha = interpolationAlpha;
                }

                _projectileContext.Interpolate = interpolateProjectile;
                _projectileContext.InterpolationData = interpolationData;

                // If barrel transform is not available anymore (e.g. weapon was switched before projectile finished)
                // let's use at least some dummy (first) one. Doesn't matter at this point much.
                int barrelTransformIndex = data.WeaponAction < barrelTransformCount ? data.WeaponAction : 0;
                _projectileContext.BarrelTransform = weaponBarrelTransforms[barrelTransformIndex];

                projectile.OnRender(_projectileContext, ref data);
            }

            if (projectile.IsFinished == true) {
                DestroyProjectile(_projectileContext,projectile);
                _visibleProjectiles[i] = null;
            }
        }

        _visibleProjectileCount = _projectileCount;
    }



    private Projectile CreateProjectile(ProjectileContext context, ref ProjectileData data) {

        var projectile = context.Cache.Get(_projectilePrefabs[data.PrefabId]);

        Runner.MoveToRunnerScene(projectile);

        projectile.Activate(context, ref data);

        return projectile;
    }

    private void DestroyProjectile(ProjectileContext context, Projectile projectile) {
        projectile.Deactivate(_projectileContext);

        //Context.ObjectCache.Return(projectile.gameObject);
        context.Cache.Return(projectile.gameObject);


        //projectile.gameObject.SetActive(false);
        

    }
}
