using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerScoreItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _killsText;
    [SerializeField] private TextMeshProUGUI _deathsText;
    [SerializeField] private TextMeshProUGUI _assistsText;

    public PlayerScore PlayerScore { get => _playerScore; private set {; } }
    public string Nickname { get => _nicknameText.text; private set {; } }

    private PlayerScore _playerScore;
    
    public void Init(PlayerScore playerScpre)
    {
        _playerScore = playerScpre;

        SetAll();
        SubscribeToPlayerScoreEvents();
    }
    private void OnDestroy() {
        
        // Asigura-te ca asta e apelata ianinte ca PlayerScore din RoomPlayer.localPlayer sa fie distrus
        UnsubscribeFromPlayerScoreEvents();
    }

    private void SetKills() {
        _killsText.text = "K: " + _playerScore.Kills;
    }
    private void SetDeaths() {
        _deathsText.text = "D: " + _playerScore.Deaths;
    }
    private void SetAssists() {
        _assistsText.text = "A: " + _playerScore.Assists;
    }
    private void SetNickname() {

        _nicknameText.text = ClientInfo.Username;
    }
    private void SetAll() {

        SetKills();
        SetDeaths();
        SetAssists();
        SetNickname();
    }

    private void SubscribeToPlayerScoreEvents() {

        _playerScore.OnKillsChanged += SetKills;
        _playerScore.OnDeathsChanged += SetDeaths;
        _playerScore.OnAssistsChanged += SetAssists;
    }

    private void UnsubscribeFromPlayerScoreEvents() {

        _playerScore.OnKillsChanged -= SetKills;
        _playerScore.OnDeathsChanged -= SetDeaths;
        _playerScore.OnAssistsChanged -= SetAssists;
    }
}
