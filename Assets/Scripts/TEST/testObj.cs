using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testObj : NetworkBehaviour
{
  

    public override void Spawned() {
        base.Spawned();

        Debug.Log("Spawned test");

        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
    }

    public override void Despawned(NetworkRunner runner, bool hasState) {
        base.Despawned(runner, hasState);

        Debug.Log("Despawned test");

    }
}
