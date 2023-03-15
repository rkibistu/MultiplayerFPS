using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileUtility
{
    public static bool ProjectileCast(NetworkRunner runner, PlayerRef owner, Vector3 firePosition, Vector3 direction, float distance, LayerMask hitMask, out LagCompensatedHit hit) {
        
        var hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
        return runner.LagCompensation.Raycast(firePosition, direction, distance, owner, out hit, hitMask, hitOptions);
    }
}
