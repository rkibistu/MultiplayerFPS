using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 
/// Informatiile folositoare pentru sever.
/// Stocare in memoria persistenta oferita ded unity
/// 
/// Se apeleaza cand informatiile sunt actualizate din cadrul menu-ului,
/// 
/// </summary>

public static class ServerInfo {

    public const int UserCapacity = 8; //the actual hard limit

    public static string LobbyName;
    public static string MapName => ResourceManager.Instance.mapDefinitions[MapId].mapName;

    public static int GameMode {
        get => PlayerPrefs.GetInt("S_GameMode", 0);
        set => PlayerPrefs.SetInt("S_GameMode", value);
    }

    public static int MapId {
        get => PlayerPrefs.GetInt("S_TrackId", 0);
        set => PlayerPrefs.SetInt("S_TrackId", value);
    }

    public static int MaxUsers {
        get => PlayerPrefs.GetInt("S_MaxUsers", 4);
        set => PlayerPrefs.SetInt("S_MaxUsers", Mathf.Clamp(value, 1, UserCapacity));
    }
}