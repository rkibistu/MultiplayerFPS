using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// Pe menu-ul RoomPlayerUI
/// 
/// Actualizeaza menu-ul cand playerii apasa butonul de Ready 9adauga acea bifa de confirmare)
/// 
/// </summary>

public class LobbyItemUI : MonoBehaviour {

    public TextMeshProUGUI username;
    public Image ready;
    public Image leader;

    private RoomPlayer _player;

    public void SetPlayer(RoomPlayer player) {
        _player = player;
    }

    private void Update() {
        if (_player.Object != null && _player.Object.IsValid) {
            username.text = _player.Username.Value;
            ready.gameObject.SetActive(_player.IsReady);
        }
    }
}