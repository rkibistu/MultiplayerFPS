using UnityEngine;

/// <summary>
/// 
/// Informatii specifice clientilor individuali.
/// Stocare in memoria persistenta oferita ded unity
/// 
/// </summary>

public static class ClientInfo {
    public static string Username {
        get => PlayerPrefs.GetString("C_Username", string.Empty);
        set => PlayerPrefs.SetString("C_Username", value);
    }


    public static string LobbyName {
        get => PlayerPrefs.GetString("C_LastLobbyName", "");
        set => PlayerPrefs.SetString("C_LastLobbyName", value);
    }
}