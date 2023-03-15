using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConsumablesSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] _spawnPoints;


    [SerializeField]
    private GameObject[] _consumablesPrefab;

    private void Start() {
        
        foreach(var prefab in _consumablesPrefab) {

            var obj = Context.Instance.ObjectCache.Get(prefab);
            

        }
    }
}
