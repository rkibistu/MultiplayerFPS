using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : NetworkBehaviour
{
    [SerializeField] private float _speed = 6;

    private NetworkCharacterControllerPrototype _cc;
    private Vector3 _forward;
    [Networked] private TickTimer delay { get; set; }


    [SerializeField]
    private Projectile _projectilePrefab;
    [SerializeField]
    private ProjectileManager _projectileManager;


    private void Awake() {
        _cc = GetComponent < NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    public override void FixedUpdateNetwork() {

    

        if (GetInput(out NetworkInputData data)) {

            data.direction.Normalize();
            _cc.Move(_speed * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (delay.ExpiredOrNotRunning(Runner)) {

                if ((data.buttons & NetworkInputData.MOUSEBUTTON0) != 0) {
                    delay = TickTimer.CreateFromSeconds(Runner, 2f);

                    if (_projectilePrefab != null) {

                        _projectileManager.AddProjectile(_projectilePrefab, transform.position, _forward, 0);
                    }
                }

            }
        }

    }


    public override void Spawned() {
        base.Spawned();

        _projectileManager.OnSpawned();
    }

    private void LateUpdate() {

        _projectileManager.OnFixedUpdate();

        Transform[] tr = new Transform[1];
        tr[0] = transform;
        _projectileManager.OnRender(tr);
    }
}
