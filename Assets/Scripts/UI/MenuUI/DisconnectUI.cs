using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// Ofera posibilitatea de a afisa un promt cu mesaj specific in cazul problemelor de conexiune 
/// 
/// </summary>
public class DisconnectUI : MonoBehaviour {
    public Transform parent;
    public TextMeshProUGUI disconnectStatus;
    public TextMeshProUGUI disconnectMessage;

    public void ShowMessage(string status, string message) {
        if (status == null || message == null)
            return;

        disconnectStatus.text = status;
        disconnectMessage.text = message;

        Debug.Log($"Showing message({status},{message})");
        parent.gameObject.SetActive(true);
    } 
}