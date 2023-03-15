using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHitAction : byte {
    None,
    Damage,
    Heal,
}

public enum EHitType {
    None,
    Projectile,
    Explosion,
    Suicide,
}

public struct HitData {
    public EHitAction Action;
    public float Amount;
    public bool IsFatal;
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Normal;
    public PlayerRef InstigatorRef;
    public IHitInstigator Instigator;
    public IHitTarget Target;
    public EHitType HitType;
}
public interface IHitTarget {
    bool IsActive { get; }
    Transform HeadPivot { get; }
    Transform BodyPivot { get; }
    Transform GroundPivot { get; }

    GameObject GameObject { get; }

    void ProcessHit(ref HitData hit);
}

public interface IHitInstigator {
    void HitPerformed(HitData hit);
}

public static class HitUtility {
    // PUBLIC METHODS

    public static HitData ProcessHit(PlayerRef instigatorRef, Vector3 direction, LagCompensatedHit hit, float baseDamage, EHitType hitType) {
        var target = GetHitTarget(hit.Hitbox, hit.Collider);
        if (target == null)
            return default;

        HitData hitData = default;

        hitData.Action = EHitAction.Damage;
        hitData.Amount = baseDamage;
        hitData.Position = hit.Point;
        hitData.Normal = hit.Normal;
        hitData.Direction = direction;
        hitData.Target = target;
        hitData.InstigatorRef = instigatorRef;
        hitData.HitType = hitType;

        return ProcessHit(ref hitData);
    }

    public static HitData ProcessHit(ref HitData hitData) {
        hitData.Target.ProcessHit(ref hitData);

        // For local debug targets we show hit feedback immediately
        // if (hitData.Instigator != null && hitData.Target is Health == false)
        // {
        // 	hitData.Instigator.HitPerformed(hitData);
        // }

        return hitData;
    }



    private static IHitTarget GetHitTarget(Hitbox hitbox, Collider collider) {
        if (hitbox != null)
            return hitbox.Root.GetComponent<IHitTarget>();

        if (collider == null)
            return null;

        if (ObjectLayerMask.HitTargets.value.IsBitSet(collider.gameObject.layer) == false)
            return null;

        return collider.GetComponentInParent<IHitTarget>();
    }


}
