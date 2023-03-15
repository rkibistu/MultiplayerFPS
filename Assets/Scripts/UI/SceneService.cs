using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SceneService : MonoBehaviour
{
    // PUBLIC MEMBERS

    public bool IsActive => _isActive;
    public bool IsInitialized => _isInitialized;

    // PRIVATE MEMBERS

    private SceneController _scene;
    private bool _isInitialized;
    private bool _isActive;

    // INTERNAL METHODS

    // Seteaza SceneController si apeleaza OnInitlize(functie suprasrica de copii)
    internal void Initialize(SceneController scene) {
        if (_isInitialized == true)
            return;

        _scene = scene;
        OnInitialize();

        _isInitialized = true;
    }

    // Apeleaza Deactivate() si OnDeinitialize suprasrica de copii
    internal void Deinitialize() {
        if (_isInitialized == false)
            return;

        Deactivate();

        OnDeinitialize();

        _scene = null;

        _isInitialized = false;
    }

    //apeleaza Activate suprasrica de copii
    internal void Activate() {
        if (_isInitialized == false)
            return;

        if (_isActive == true)
            return;

        OnActivate();

        _isActive = true;
    }

    // Apeleaaza OnTick suprasrica de copii
    internal void Tick() {
        if (_isActive == false)
            return;

        OnTick();
    }

    // Apeleaaza OnLateTick suprasrica de copii
    internal void LateTick() {
        if (_isActive == false)
            return;

        OnLateTick();
    }

    // Apeleaza OnDeactivate suprasrica de copii
    internal void Deactivate() {
        if (_isActive == false)
            return;

        OnDeactivate();

        _isActive = false;
    }

    // SceneService INTERFACE

    protected virtual void OnInitialize() {
    }

    protected virtual void OnDeinitialize() {
    }

    protected virtual void OnActivate() {
    }

    protected virtual void OnDeactivate() {
    }

    protected virtual void OnTick() {
    }

    protected virtual void OnLateTick() {
    }
}
