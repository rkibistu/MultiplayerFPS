using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

/// <summary>
/// 
/// CreateRoomScreen -> menu specific hostului
/// 
/// Are referinte spre toate butoanele specifice acestui menu.
/// Functiile sunt apelate la interactiunea cu butoanele prezentate
///             -> la modificarea valorilor va actualiza datele din ServerInfo (folosite mai tarziu pentru creearea lumii jocului)
/// 
/// </summary>

public class CreateGameUI : MonoBehaviour {
    public TMP_InputField lobbyName;
    public TMP_Dropdown map;
    public TMP_Dropdown gameMode;
    public Slider playerCountSlider;
    public Image mapImage;
    public TextMeshProUGUI playerCountSliderText;
    public Image playerCountIcon;
    public Button confirmButton;

    //resources
    public Sprite padlockSprite, publicLobbyIcon;

    private void Start() {

        playerCountSlider.SetValueWithoutNotify(8);
        SetPlayerCount();

        map.ClearOptions();
        map.AddOptions(ResourceManager.Instance.mapDefinitions.Select(x => x.mapName).ToList());
        map.onValueChanged.AddListener(SetTrack);
        SetTrack(0);

        gameMode.ClearOptions();
        gameMode.AddOptions(ResourceManager.Instance.gameTypes.Select(x => x.ModeName).ToList());
        gameMode.onValueChanged.AddListener(SetGameType);
        SetGameType(0);

        playerCountSlider.wholeNumbers = true;
        playerCountSlider.minValue = 1;
        playerCountSlider.maxValue = 8;
        playerCountSlider.value = 2;
        playerCountSlider.onValueChanged.AddListener(x => ServerInfo.MaxUsers = (int)x);

        lobbyName.onValueChanged.AddListener(x => {
            ServerInfo.LobbyName = x;
            confirmButton.interactable = !string.IsNullOrEmpty(x);
        });
        lobbyName.text = ServerInfo.LobbyName = "Session" + Random.Range(0, 1000);

        ServerInfo.MapId = map.value;
        ServerInfo.GameMode = gameMode.value;
        ServerInfo.MaxUsers = (int)playerCountSlider.value;
    }

    public void SetGameType(int gameType) {
        ServerInfo.GameMode = gameType;
    }

    public void SetTrack(int trackId) {
        ServerInfo.MapId = trackId;
        mapImage.sprite = ResourceManager.Instance.mapDefinitions[trackId].mapIcon;
    }

    public void SetPlayerCount() {
        playerCountSlider.value = ServerInfo.MaxUsers;
        playerCountSliderText.text = $"{ServerInfo.MaxUsers}";
        playerCountIcon.sprite = ServerInfo.MaxUsers > 1 ? publicLobbyIcon : padlockSprite;
    }

    // UI Hooks

    private bool _lobbyIsValid;

    public void ValidateLobby() {
        _lobbyIsValid = string.IsNullOrEmpty(ServerInfo.LobbyName) == false;
    }

    public void TryFocusScreen(UIScreen screen) {
        if (_lobbyIsValid) {
            UIScreen.Focus(screen);
        }
    }

    public void TryCreateLobby(GameLauncher launcher) {
        if (_lobbyIsValid) {
            launcher.JoinOrCreateLobby();
            _lobbyIsValid = false;
        }
    }
}