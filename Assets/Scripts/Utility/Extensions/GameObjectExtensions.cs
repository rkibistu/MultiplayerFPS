using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameObjectExtensions
{
    public static void SetLayer(this GameObject gameObject, int layer, bool includeChildren = false) {
        if (includeChildren == false) {
            gameObject.layer = layer;
        }
        else {
            gameObject.transform.SetLayer(layer, includeChildren);
        }
    }

    public static void SetLayer(this Transform parent, int layer, bool includeChildren = false) {
        parent.gameObject.layer = layer;

        if (includeChildren == false)
            return;

        for (int i = 0, count = parent.childCount; i < count; i++) {
            parent.GetChild(i).SetLayer(layer, true);
        }
    }
}
