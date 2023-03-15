using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This allows us to call 'Awake' (b.Setup()) and 'OnDestroy' (b.OnDestruction()) on disabled MonoBehaviours.
/// Usage: inherit this class on a child and implement Setup and OnDestruction
///             MUST: a parent must have DisabledUI component atached
/// </summary>

public interface IDisabledUI {
    void Setup();
    void OnDestruction();
}
