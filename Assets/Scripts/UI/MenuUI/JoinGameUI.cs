using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// Atasat de JoinRoomScreen -> setare nume lobby pentru conectare (join)
/// 
/// Metodele sunt atasate de butoanele/campurile din acel menu,
///     la schimbarea datelor/apasarea butoanelor se actualizeaza datele din ClientInfo
///     
/// 
/// </summary>
public class JoinGameUI : MonoBehaviour {

    public TMP_InputField lobbyName;
    public Button confirmButton;

    private void OnEnable() {
        SetLobbyName(lobbyName.text);
    }

    private void Start() {
        lobbyName.onValueChanged.AddListener(SetLobbyName);
        lobbyName.text = ClientInfo.LobbyName;
    }

    private void SetLobbyName(string lobby) {
        ClientInfo.LobbyName = lobby;
        //confirmButton.interactable = !string.IsNullOrEmpty(lobby);
    }
}
