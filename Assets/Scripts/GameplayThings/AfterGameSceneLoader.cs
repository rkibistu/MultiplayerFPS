using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Obiect existent in scena incarcata la finalizarea unui meci.
/// 
/// Face setarile initiale a acestei scene.
///     -> focus menu on RoomScreen
/// 
/// </summary>

public class AfterGameSceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIScreen.activeScreen.FocusScreen(InterfaceManager.Instance.lobbyMenu);
    }
}
