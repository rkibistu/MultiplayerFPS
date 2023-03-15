using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

/// <summary>
/// 
/// Component atached to agent
/// 
/// Raycast from camera to middle of the screen(crosshair)
///     If hits a pickable item( set on pickable layers) -> return it
///     Else return null
/// 
/// </summary>

public class PickupItem : MonoBehaviour {
    [SerializeField]
    private float _distanceCheck = 10f;
    [SerializeField]
    private LayerMask _pickableLayer;
    [SerializeField]
    private LayerMask _actionableLayer;

    private Ray _ray;
    private RaycastHit _hit;

    

    public PickableItem FindItem(Camera camera) {

        _ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(_ray, out _hit, _distanceCheck, _pickableLayer)) {


            return _hit.collider.GetComponent<PickableItem>();
            
        }
 
        if (Physics.Raycast(_ray, out _hit, _distanceCheck, _actionableLayer)) {

            _hit.collider.GetComponent<LootingBox>().OpenLootBox();

        }
        return null;
    }

    private void OnDrawGizmos() {


        Gizmos.DrawRay(_ray);
    }

}
