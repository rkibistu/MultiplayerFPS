using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestNetworkRunner : MonoBehaviour
{
    // PUBLIC MEMBERS

    public NetworkRunner Runner => _runner;

    // PRIVATE MEMBERS

    [SerializeField]
    private GameObject _gameManager;


    private NetworkRunner _runner;

    private void OnGUI() {
        if (_runner == null) {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host")) {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join")) {
                StartGame(GameMode.Client);
            }
        }
    }

    async void StartGame(GameMode mode) {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;


        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs() {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        // _runner.Spawn(_gameManager);


    }
}
