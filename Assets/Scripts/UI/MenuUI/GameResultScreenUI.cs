using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Folosim asta pe obiectul GameResultScreen ca la finalul animatiei de Victory/Lose sa schimbam la ecranul de scor automat
///             deci e folsoita in aniamtion events
/// 
/// </summary>

public class GameResultScreenUI : MonoBehaviour
{
    private Animator _animator;

    private int _victoryAnim;
    private int _lostAnim;

    private void Awake() {

        _animator = GetComponent<Animator>();
        _victoryAnim = Animator.StringToHash("VictoryUI");
        _lostAnim = Animator.StringToHash("LostUI");
    }

    // called by aniamtion events, after victory/lose animation
    private void FocusToScoreScreen() {

        UIScreen.Focus(InterfaceManager.Instance.scoreScreen);
        InterfaceManager.Instance.scoreScreen.GetComponent<ScoreScreenUI>().SetBackToLobbyButtonActive();

        RoomPlayer.LocalRoomPlayer.Input.UnlockCursour();
    }

    public void PlayVictoryAnimation() {

        _animator.Play(_victoryAnim);
    }

    public void PlayLostAniamtion() {

        _animator.Play(_lostAnim);
    }
}
