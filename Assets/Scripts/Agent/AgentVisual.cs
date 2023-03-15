using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class AgentVisual : MonoBehaviour
{
    // PUBLIC MEMBERS
    [Tooltip("Avatar that represents the caracter. A face photo or somethng")]
    public Sprite Avatar;

    // PRIVATE MEMBERS
    [SerializeField]
    [Tooltip("Character model mesh which need to be changed")]
    private SkinnedMeshRenderer _characterMeshToChange;
    [SerializeField]
    [Tooltip("Mesh used for proxies")]
    private Mesh _characterMesh;
    [SerializeField]
    [Tooltip("Mesh used for local player")]
    private Mesh _headlessCharacterMesh;
    



    // PUBLIC METHODS

    public void SetVisibility(bool isVisible) {

        _characterMeshToChange.sharedMesh = (isVisible == true) ? _characterMesh : _headlessCharacterMesh;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.H)) {
            SetVisibility(false);
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            SetVisibility(true);
        }
    }


}
