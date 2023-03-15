using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Atasat de un obiect din context (context - singleton)
/// Folosit pentru a avea acces global la MainCamera (setata in editor)
/// </summary>

public class CameraContext : MonoBehaviour
{
   
    private void Awake() {


        Debug.Log("Register CameraContext to context");
        Context.Instance.CameraContext = this;
    }

    // PUBLIC MEMEBERS

    public Camera Camera => _camera;

    // PRIVATE MEMEBERS

    [SerializeField]
    private Camera _camera;
}
