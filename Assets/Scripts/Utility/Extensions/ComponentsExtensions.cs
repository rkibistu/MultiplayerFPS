using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ComponentsExtensions
{
    public static void SetActive(this Component component, bool value) {
        if (component == null)
            return;

        if (component.gameObject.activeSelf == value)
            return;

        component.gameObject.SetActive(value);
    }
}
