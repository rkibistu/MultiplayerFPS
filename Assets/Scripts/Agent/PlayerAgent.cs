using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerAgent : Agent
{
    // PUBLIC MEMEBRS
    public event Action<PlayerAgent> AgentDespawned;
    public Weapons Weapons => _weapons;

    public Health Health => _health;

    // PRIVATE MEMEBRS

    private GameObject _mainCamera;
    private Weapons _weapons;
    private Health _health;

    private bool _kccHasPositionToSet = false;
    private Vector3 _kccPositionToSet;

    // PROTECTED MEMEBRS

    protected GameObject MainCamera => _mainCamera;

    // MONOBEHAVIOUR

    protected override void Awake() {
        base.Awake();

        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (_mainCamera == null)
            Debug.LogError("Main Camera is missing from the scene!");


        _weapons = GetComponent<Weapons>();
        _health = GetComponent<Health>();
    }

    // PUBLIC METHODS

    // KCC.SetPosition trebuie apleata din FixedUpdateNetwork pentru a mentrine modificarile
    // aceasta functie e apelata oricand pentru a pregati tereneul, astfel incat in primul FixedUpdateNetwork sa se seteze pozitia
    public void MoveTo(Vector3 position) {

        //KCC.SetPosition(position);

        _kccHasPositionToSet = true;
        _kccPositionToSet = position;
    }

    // Networkbehaviour INTERFACE

    protected override void OnSpawned() {
        base.OnSpawned();

        Weapons.OnSpawned();
    }

    protected override void OnDespawned() {
        base.OnDespawned();

        Weapons.OnDespawned();

        AgentDespawned?.Invoke(this);
        AgentDespawned = null;
    }

    // PROTECTED METHODS

    // Aplica schimbarile setate in functia public MoveTo
    // functia aceasta trebuie neaparat apela in FixedUpdate(Network)
    protected void DoMoveTo() {

        if (_kccHasPositionToSet == false)
            return;

        KCC.SetPosition(_kccPositionToSet);
        _kccHasPositionToSet = false;
    }
}
