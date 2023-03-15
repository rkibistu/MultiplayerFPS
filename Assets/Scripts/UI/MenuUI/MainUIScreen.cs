using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Menu-ul principal ce apare la pornirea aplicatiei
/// 
/// </summary>

public class MainUIScreen : MonoBehaviour {
    void Awake() {
        UIScreen.Focus(GetComponent<UIScreen>());
    }
}
