using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// Pe menu-ul RoomScreen
/// 
/// Prezinta informatiile alese la crearea Room-ului (harta, gametype, nume)
/// Prezinta lista de playeri conectati + starea lor
/// 
/// In momentul in care toti playerii sunt gata -> incarca noua scena apeland metode din LevelManager
/// 
/// </summary>
public class LobbyUI : MonoBehaviour, IDisabledUI {
    public GameObject textPrefab;
    public Transform parent;
    public Button readyUp;
    public TextMeshProUGUI mapNameText;
    public TextMeshProUGUI modeNameText;
    public TextMeshProUGUI lobbyNameText;
    public TMP_Dropdown mapNameDropdown;
    public TMP_Dropdown gameTypeDropdown;
    public Image mapIconImage;

    private static readonly Dictionary<RoomPlayer, LobbyItemUI> ListItems = new Dictionary<RoomPlayer, LobbyItemUI>();
    private static bool IsSubscribed;

    private void Awake() {
        mapNameDropdown.onValueChanged.AddListener(x => {
            var gm = GameManager.Instance;
            if (gm != null) gm.MapId = x;
        });
        gameTypeDropdown.onValueChanged.AddListener(x => {
            var gm = GameManager.Instance;
            if (gm != null) gm.GameTypeId = x;
        });

        GameManager.OnLobbyDetailsUpdated += UpdateDetails;

        RoomPlayer.PlayerChanged += (player) => {
            var isLeader = RoomPlayer.LocalRoomPlayer.IsLeader;
            mapNameDropdown.interactable = isLeader;
            gameTypeDropdown.interactable = isLeader;
        };
    }

    private void OnEnable() {


        if (RoomPlayer.LocalRoomPlayer != null && RoomPlayer.LocalRoomPlayer.HasStateAuthority) {

            RoomPlayer.LocalRoomPlayer.SetAllPlayersReadyState(false);
        }
    }

    void UpdateDetails(GameManager manager) {
        lobbyNameText.text = "Room Code: " + manager.LobbyName;
        mapNameText.text = manager.MapName;
        modeNameText.text = manager.ModeName;

        var tracks = ResourceManager.Instance.mapDefinitions;
        var trackOptions = tracks.Select(x => x.mapName).ToList();

        var gameTypes = ResourceManager.Instance.gameTypes;
        var gameTypeOptions = gameTypes.Select(x => x.ModeName).ToList();

        mapNameDropdown.ClearOptions();
        mapNameDropdown.AddOptions(trackOptions);
        mapNameDropdown.value = GameManager.Instance.MapId;

        mapIconImage.sprite = ResourceManager.Instance.mapDefinitions[GameManager.Instance.MapId].mapIcon;

        gameTypeDropdown.ClearOptions();
        gameTypeDropdown.AddOptions(gameTypeOptions);
        gameTypeDropdown.value = GameManager.Instance.GameTypeId;
    }

    public void Setup() {

        Debug.Log("Setup lobby");

        if (IsSubscribed) return;

        RoomPlayer.PlayerJoined += AddPlayer;
        RoomPlayer.PlayerLeft += RemovePlayer;

        RoomPlayer.PlayerChanged += EnsureAllPlayersReady;

        readyUp.onClick.AddListener(ReadyUpListener);

        IsSubscribed = true;
    }

    private void OnDestroy() {
        if (!IsSubscribed) return;

        RoomPlayer.PlayerJoined -= AddPlayer;
        RoomPlayer.PlayerLeft -= RemovePlayer;

        readyUp.onClick.RemoveListener(ReadyUpListener);

        IsSubscribed = false;
    }

    private void AddPlayer(RoomPlayer player) {
        if (ListItems.ContainsKey(player)) {
            var toRemove = ListItems[player];
            Destroy(toRemove.gameObject);

            ListItems.Remove(player);
        }

        var obj = Instantiate(textPrefab, parent).GetComponent<LobbyItemUI>();
        obj.SetPlayer(player);

        ListItems.Add(player, obj);

        UpdateDetails(GameManager.Instance);
    }

    private void RemovePlayer(RoomPlayer player) {
        if (!ListItems.ContainsKey(player))
            return;

        var obj = ListItems[player];
        if (obj != null) {
            Destroy(obj.gameObject);
            ListItems.Remove(player);
        }
    }

    public void OnDestruction() {
    }

    private void ReadyUpListener() {
        var local = RoomPlayer.LocalRoomPlayer;
        if (local && local.Object && local.Object.IsValid) {
            local.RPC_ChangeReadyState(!local.IsReady);
        }
    }

    private void EnsureAllPlayersReady(RoomPlayer lobbyPlayer) {
        if (!RoomPlayer.LocalRoomPlayer.IsLeader)
            return;

        if (IsAllReady()) {
            int scene = ResourceManager.Instance.mapDefinitions[GameManager.Instance.MapId].buildIndex;
            LevelManager.LoadTrack(scene);
        }
    }

    private static bool IsAllReady() => RoomPlayer.Players.Count > 0 && RoomPlayer.Players.All(player => player.IsReady);
}