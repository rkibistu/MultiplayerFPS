using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ScoreScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject _listHolder;
    [SerializeField] private GameObject _playerScoreItemPrefab;

    [SerializeField] private TextMeshProUGUI _winnerText;

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
        foreach(Transform child in _listHolder.transform) {

            Destroy(child);
        }

        //add all current players to list
        foreach (var player in RoomPlayer.Players) {


            var obj = Instantiate(_playerScoreItemPrefab, _listHolder.transform).GetComponent<PlayerScoreItemUI>();
            obj.Init(player.PlayerScore);
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

        int index = 0;
        foreach (var playerScore in playersScoreUI) {

            playerScore.transform.SetSiblingIndex(index++);
        }
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.L)) {

            ShowcaseWinner();
        }
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
}
