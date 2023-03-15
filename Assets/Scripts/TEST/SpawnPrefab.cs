using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{

    public GameObject prefab;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {

        Instantiate(prefab, parent);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
