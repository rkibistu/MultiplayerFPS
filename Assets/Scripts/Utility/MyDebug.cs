using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug : MonoBehaviour {

    [SerializeField]
    static private bool _active = false;

    static public void SetDebug(bool active) {

        _active = active;
    }

    static public void Log(object message) {

        if (!_active)
            return;

        Debug.Log(message);
    }
}
