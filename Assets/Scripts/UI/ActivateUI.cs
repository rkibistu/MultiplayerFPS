using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Activeaza/Dezactiveaza UI specific gameplay-ului
/// 
/// </summary>
public class ActivateUI : MonoBehaviour
{
    private GameplaySceneController _gameplayScene;
    private bool _enabled = false;

    public void Toggle() {

        if (_enabled == false)
            Activate();
        else
            Dezactivate();
    }

    public void Activate() {

        if (_enabled == true)
            return;

        StartCoroutine(ActivateCourutine());
    }

    public void Dezactivate() {

        if (_enabled == false)
            return;

        if (_gameplayScene != null)
            _gameplayScene = GameObject.FindObjectOfType<GameplaySceneController>(true);

        StopCoroutine(ActivateCourutine());

        // dezactivate scene Ui
        _gameplayScene.Deactivate();
        _gameplayScene.Deinitialize();
        _gameplayScene.SetActive(false);

        _enabled = false;
    }

    private IEnumerator ActivateCourutine() {

        while(_gameplayScene == null) {

            _gameplayScene = GameObject.FindObjectOfType<GameplaySceneController>(true);
            yield return null;
        }

        _gameplayScene.SetActive(true);
        _gameplayScene.Initialize();
        _gameplayScene.Activate();

        _enabled = true;


        foreach(var roomPlayer in RoomPlayer.Players) {

            roomPlayer.ActiveAgent._gameplaySceneController = _gameplayScene;
        }
        
    }
}
