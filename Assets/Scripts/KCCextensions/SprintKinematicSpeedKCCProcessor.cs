using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.KCC;


public class SprintKinematicSpeedKCCProcessor : KCCProcessor, IKinematicSpeedKCCProcessor {
    // PRIVATE MEMBERS

    [SerializeField]
    private float _kinematicSpeedMultiplier = 1.5f;

    // PUBLIC METHODS

    // This method is used by all processors modifying kinematic speed to ensure there is a consistent priority calculation.
    // In this case processor priority equals to multiplier. Processor with highest multiplier will be executed first, providing "highest speedup available".
    public static float GetProcessorPriority(float multiplier) {
        return multiplier;
    }

    // KCCProcessor INTERFACE

    public override float Priority => GetProcessorPriority(_kinematicSpeedMultiplier);

    public override EKCCStages GetValidStages(KCC kcc, KCCData data) {
        // Only SetKinematicSpeed stage is used, rest are filtered out and corresponding method calls will be skipped.
        return EKCCStages.SetKinematicSpeed;
    }

    public override void SetKinematicSpeed(KCC kcc, KCCData data) {

        if (data.Sprint) {

            // Apply multiplier.
            data.KinematicSpeed *= _kinematicSpeedMultiplier;

            // Suppress all other processors in same category (identified by the interface) with lower priority.
            kcc.SuppressProcessors<IKinematicSpeedKCCProcessor>();
        }
    }
}
