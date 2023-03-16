using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject _listHolder;
    [SerializeField] private GameObject _playerScoreItemPrefabBlue;
    [SerializeField] private GameObject _playerScoreItemPrefabRed;

    [SerializeField] private TextMeshProUGUI _winnerText;
    [SerializeField] private Transform _footer;

    [SerializeField] private Button _backToLobbyButton;

    private GameObject _currentPlayerScoreItemPrefab;
    

    private void Start() {

        Debug.Log("Start ScoreScreenUI");
        PopulateScoreList();
    }

    private void OnEnable() {

        OrderScoresByKills(); 
    }
    private void PopulateScoreList() {

        //clear lsit
        _winnerText.SetActive(false);
        ClearScoreList();

        //add all current players to list
        foreach (var player in RoomPlayer.Players) {

            TogglePlayerScoreitemPrefab();

            var obj = Instantiate(_currentPlayerScoreItemPrefab, _listHolder.transform).GetComponent<PlayerScoreItemUI>();
            obj.Init(player.PlayerScore);
        }

        OrderScoresByKills();
    }

    private void ClearScoreList() {
        if (_listHolder.transform.childCount > 2) {
            for (int i = 1; i < _listHolder.transform.childCount - 1; i++) {

                Destroy(_listHolder.transform.GetChild(i));
            }
        }
    }

    // Order PlayerScoreItemUI care sunt aflate in lista de scoruri din listHolder
    //  Functia ar trb mereu apelata cand afisam score screen-ul
    private void OrderScoresByKills() {

       
        var playersScoreUI = _listHolder.GetComponentsInChildren<PlayerScoreItemUI>();
        
        for(int i=0;i<playersScoreUI.Length - 1; i++) {
            for (int j = i + 1; j < playersScoreUI.Length; j++) {

                if (playersScoreUI[i].PlayerScore.Kills < playersScoreUI[j].PlayerScore.Kills) {

                    var temp = playersScoreUI[i];
                    playersScoreUI[i] = playersScoreUI[j];
                    playersScoreUI[j] = temp;
                }
            }
        }

        int index = 1;
        foreach (var playerScore in playersScoreUI) {

            playerScore.transform.SetSiblingIndex(index++);
        }

        _footer.SetAsLastSibling();
    }

    private void ShowcaseWinner() {


        var playersScoreUI = _listHolder.GetComponentsInChildren<RectTransform>();
        playersScoreUI[1].sizeDelta = new Vector2(playersScoreUI[1].sizeDelta.x, playersScoreUI[1].sizeDelta.y + 20);
        for (int i = 2; i < playersScoreUI.Length; i++) {

            playersScoreUI[i].sizeDelta = new Vector2(playersScoreUI[i].sizeDelta.x, playersScoreUI[i].sizeDelta.y);
        }

        _winnerText.SetActive(true);
        _winnerText.text = "WINNER: " + playersScoreUI[1].GetComponent<PlayerScoreItemUI>().Nickname;
    }

    private void TogglePlayerScoreitemPrefab() {

        if(_currentPlayerScoreItemPrefab == null) {
            _currentPlayerScoreItemPrefab = _playerScoreItemPrefabBlue;
            return;
        }

        _currentPlayerScoreItemPrefab = (_currentPlayerScoreItemPrefab == _playerScoreItemPrefabBlue) ? _playerScoreItemPrefabRed : _playerScoreItemPrefabBlue;
    }

    public void SetBackToLobbyButtonActive(bool active = true) {

        _backToLobbyButton.SetActive(active);
    }

    // method called when pressing the button at the end of the round, under the score screen
    //  this method is going to despawn gameplay objects and go back to lobby
    public void BackToLobby() {


        Context.Instance.Gameplay.EndRound();
        SetBackToLobbyButtonActive(false);
    }
}
