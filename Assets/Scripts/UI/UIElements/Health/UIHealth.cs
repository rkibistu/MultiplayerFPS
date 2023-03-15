
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIHealth : UIBehaviour {
    // PRIVATE MEMBERS

    [SerializeField]
    private UIValue _healthValue;
    [SerializeField]
    private Image _healthBarFill;
    [SerializeField]
    private TextMeshProUGUI _nickaname;
    [SerializeField]
    private Image _avatar;
    protected override void Start() {

        _nickaname.text = ClientInfo.Username;
        _avatar.sprite = RoomPlayer.LocalRoomPlayer.ActiveAgent.GetComponent<AgentVisual>().Avatar;
    }

    // PUBLIC METHODS

    public void UpdateHealth(Health health) {
        _healthValue.SetValue(health.CurrentHealth, health.MaxHealth);

        _healthBarFill.fillAmount = health.CurrentHealth/health.MaxHealth;
    }
}