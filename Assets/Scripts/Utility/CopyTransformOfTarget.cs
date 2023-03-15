using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformOfTarget : MonoBehaviour
{

    [SerializeField]
    private Transform target;
    void Update()
    {

        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
