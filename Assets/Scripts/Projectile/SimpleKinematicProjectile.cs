using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleKinematicProjectile : KinematicProjectile
{

    [SerializeField]
    private float _damage = 10f;
    [SerializeField]
    private EHitType _hitType = EHitType.Projectile;
    [SerializeField]
    private LayerMask _hitMask;


   

    // KinematicProjectile INTERFACE

    public override void OnFixedUpdate(ProjectileContext context, ref ProjectileData data) {
        var previousPosition = GetMovePosition(context.Runner, ref data, context.Runner.Tick - 1);
        var nextPosition = GetMovePosition(context.Runner, ref data, context.Runner.Tick);

        var direction = nextPosition - previousPosition;
        float distance = direction.magnitude;

        // Normalize
        direction /= distance;

        

        if (_length > 0f) {
            float elapsedDistanceSqr = (previousPosition - data.FirePosition).sqrMagnitude;
            float projectileLength = elapsedDistanceSqr > _length * _length ? _length : Mathf.Sqrt(elapsedDistanceSqr);
            previousPosition -= direction * projectileLength;
            distance += projectileLength;
        }

        if (ProjectileUtility.ProjectileCast(context.Runner, context.InputAuthority, previousPosition, direction, distance, _hitMask, out LagCompensatedHit hit) == true) {
            HitUtility.ProcessHit(context.InputAuthority, direction, hit, _damage, _hitType);

            data.ImpactPosition = hit.Point;

            data.IsFinished = true;
        }

        base.OnFixedUpdate(context, ref data);
    }


    protected override Vector3 GetRenderPosition(ProjectileContext context, ref ProjectileData data) {
        return GetMovePosition(context.Runner, ref data, context.FloatTick);
    }

    // PRIVATE METHODS

    private Vector3 GetMovePosition(NetworkRunner runner, ref ProjectileData data, float currentTick) {
        float time = (currentTick - data.FireTick) * runner.DeltaTime;

        if (time <= 0f)
            return data.FirePosition;

        return data.FirePosition + data.FireVelocity * time;
    }
}
