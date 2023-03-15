using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Folosit ca UnityEvent in cadrul clasei HoverAction atasate de butoanele din menu-ul PlayerModeScreen
/// pentru a selecta imaginea dorita sa fie afisata
///         (host, join)
/// 
/// </summary>


public class SpotlightBridge : MonoBehaviour {
    public string target = "";

    // target - este numele SpotlightGroup-ului in care dorim sa efectuam modificarea (vezi clasa SpotlightGroup)
    // index - copilul, din acel grup, care vrem sa devina activ (sa fie focusat)

    public void FocusIndex(int index) {
        if (string.IsNullOrEmpty(target)) {
            Debug.LogWarning("SpotlightBridge target field has not been set", this);
            return;
        }

        if (SpotlightGroup.Search(target, out SpotlightGroup spotlight))
            spotlight.FocusIndex(index);
    }
}
