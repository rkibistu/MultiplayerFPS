using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// Pe menu-ul InfoSetupScreen -> pentru a actualiza profilul clientului
/// 
/// </summary>

public class ProfileSetupUI : MonoBehaviour {
    public TMP_InputField nicknameInput;
    public Button confirmButton;

    private void Start() {
        // modifica in unity Prefs atunci cand clientul isi schimba numele
        nicknameInput.onValueChanged.AddListener(x => ClientInfo.Username = x);
        nicknameInput.onValueChanged.AddListener(x => {
            // disallows empty usernames to be input
            confirmButton.interactable = !string.IsNullOrEmpty(x);
        });

        //preia numele din Unity Prefs si seteaza-l (Ultimul setat de cleint practic)
        nicknameInput.text = ClientInfo.Username;
    }

    public void AssertProfileSetup() {
        // daca nu e setat numele in Unity Prefs -> activeaza ecranul de setat numele

        if (string.IsNullOrEmpty(ClientInfo.Username))
            UIScreen.Focus(GetComponent<UIScreen>());
    }
}