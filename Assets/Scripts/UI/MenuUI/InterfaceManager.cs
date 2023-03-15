using FusionExamples.Utility;
using UnityEngine;


public class InterfaceManager : MonoBehaviour {

    // config Profile setup if neeede, set volumes, open pause menu, and quit app

    [SerializeField] private ProfileSetupUI profileSetup;

    public UIScreen mainMenu;
    public UIScreen pauseMenu;
    public UIScreen lobbyMenu;
    public UIScreen dummyScreen;
    public UIScreen scoreScreen;

    public static InterfaceManager Instance => Singleton<InterfaceManager>.Instance;

    private void Start() {
        // porneste fereastra de setat numele daca nu e setat
        //daca e setat, il preia din registrii (unity prefs)
        profileSetup.AssertProfileSetup();
    }

    public void OpenPauseMenu() {
        // open pause menu only if the kart can drive and the menu isn't open already
        if (UIScreen.activeScreen != pauseMenu) {
            UIScreen.Focus(pauseMenu);
        }
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    // Audio Hooks
    public void SetVolumeMaster(float value) => AudioManager.SetVolumeMaster(value);
    public void SetVolumeSFX(float value) => AudioManager.SetVolumeSFX(value);
    public void SetVolumeUI(float value) => AudioManager.SetVolumeUI(value);
    public void SetVolumeMusic(float value) => AudioManager.SetVolumeMusic(value);
}