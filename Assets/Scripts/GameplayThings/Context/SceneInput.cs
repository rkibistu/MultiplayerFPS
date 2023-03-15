using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Folosit pentru a bloca/debloca cursorul.
/// 
/// Poate fi accesata global prin intermediul Context.
/// 
/// </summary>

public class SceneInput : MonoBehaviour
{
    private void Awake() {

        Debug.Log("Register SceneInput to context");
        Context.Instance.SceneInput = this;
    }

    public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;

    private static int _cursorLockRequests;

    // PUBLIC METHODS

    public void RequestCursorLock() {
        // Static requests count is used for multi-peer setup
        _cursorLockRequests++;

        if (_cursorLockRequests == 1) {
            // First lock request, let's lock
            SetLockedState(true);
        }
    }
    public void RequestCursorRelease() {
        _cursorLockRequests--;
        _cursorLockRequests = Mathf.Max(0, _cursorLockRequests);

        Assert.Check(_cursorLockRequests >= 0, "Cursor lock requests are negative, this should not happen");

        if (_cursorLockRequests == 0) {
            SetLockedState(false);
        }
    }

    // PRIVATE METHODS

    private void SetLockedState(bool value) {
        Cursor.lockState = value == true ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;

        //Debug.Log($"Cursor lock state {Cursor.lockState}, visibility {Cursor.visible}");
    }

}
