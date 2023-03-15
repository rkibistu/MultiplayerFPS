using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security;

/// <summary>
/// 
///  Attached to prefab that is spawned when someone dies
///  This is goint to be spawned by KillAnnouncer every time someone dies
/// 
/// </summary>

public class KillAnnouncementUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _leftText;
    [SerializeField]
    private TextMeshProUGUI _rightText;
    [SerializeField]
    private Image _middleImage;

    
    public void SetKillAnnouncement(string leftText = null, string rightText = null, Sprite image = null) {

        if (leftText != null)
            _leftText.text = leftText;

        if (rightText != null)
            _rightText.text = rightText;

        if (image != null)
            _middleImage.sprite = image;
    }
}
