using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : NetworkBehaviour {

    public NetworkObject prefab;

    private NetworkObject spawned;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {

            spawned = Context.Instance.Runner.Spawn(prefab);
        }
        if (Input.GetKeyDown(KeyCode.L)) {

            Context.Instance.Runner.Despawn(spawned);
        }
    }

  
  
}
