using Fusion;
using Fusion.KCC;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Test : NetworkBehaviour {


    [Networked]
    public int x { get; set; }


    public override void FixedUpdateNetwork() {
        base.FixedUpdateNetwork();

    

        
    }


    private void Awake() {


        var beforeUpdater = GetComponent<BeforeHitboxManagerUpdater>();
        beforeUpdater.SetDelegates(EarlyFixedUpdate, EarlyRender);
    }

    private void EarlyFixedUpdate() {
        if (HasInputAuthority) {

            if (Context.Instance.Runner.IsForward) {

                Debug.Log($"[{Context.Instance.Runner.Tick}] Forward: " + x);
            }
            else {

                Debug.Log($"[{Context.Instance.Runner.Tick}] Resimul: " + x);
            }
        }

        x += 10;
    }

    private void EarlyRender() {
       
    }

}
