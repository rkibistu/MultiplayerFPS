using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipablePickable : PickableItem
{
    [SerializeField]
    [Tooltip("The weapon prefab to add to the agent")]
    private GameObject _equipablePrefab;
    public GameObject GetPrefabAndDestroy() {

        SelfDestroy();
        
        return _equipablePrefab;
    }


}
