using UnityEngine;

/// <summary>
/// 
/// Atasat de orice obiect ce reprezinta o pagina de menu
///
/// Garanteaza ca avem activ un singur obiect de contine aceasta componenta 
///         retine cel ativ in membru static activeScreen
/// Ofera posibilitatea schimbarii obiectului activ (functia GocusScreen)
///     Foarte des setata in editor la apasarea de butoane -> pentru manevrarea prin menu
/// 
/// 
/// </summary>

public class UIScreen : MonoBehaviour {
    public bool isModal = false;
    [SerializeField] private UIScreen previousScreen = null;

    public static UIScreen activeScreen;



    // Class Methods

    public static void Focus(UIScreen screen) {
        if (screen == activeScreen)
            return;

        if (activeScreen)
            activeScreen.Defocus();
        screen.previousScreen = activeScreen;
        activeScreen = screen;
        screen.Focus();
    }

    // merge inapoi(recursiv) din previosScreen inpreviosScreen; pana ajunge la primul sau la un Loop
    public static void BackToInitial() {
        activeScreen?.BackTo(null);
    }

    // activeaza MainMenuScreen
    // Folosit in butonul de Leave din RoomScreen
    public static void BackToMainMenu() {

        Focus(InterfaceManager.Instance.mainMenu);
    }


    // Instance Methods

    public void FocusScreen(UIScreen screen) {
        Focus(screen);
    }

    private void Focus() {
        if (gameObject)
            gameObject.SetActive(true);
    }

    private void Defocus() {
        if (gameObject)
            gameObject.SetActive(false);
    }

    public void Back() {
        if (previousScreen) {
            Defocus();
            activeScreen = previousScreen;
            activeScreen.Focus();
            previousScreen = null;
        }
    }

    public void BackTo(UIScreen screen) {
        while (activeScreen != null && activeScreen.previousScreen != null && activeScreen != screen)
            activeScreen.Back();
    }

}