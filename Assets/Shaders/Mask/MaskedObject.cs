using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskedObject : MonoBehaviour
{
    private void Start() {

        GetComponent<SkinnedMeshRenderer>().material.renderQueue = 3002;
    }
}
