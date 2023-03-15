using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocalInput : MonoBehaviour
{

    private void Update() {

        ShowScoreScreen();
    }

    private static void ShowScoreScreen() {

        if (Input.GetKeyDown(KeyCode.Tab)) {

            UIScreen.Focus(InterfaceManager.Instance.scoreScreen);
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {

            UIScreen.Focus(InterfaceManager.Instance.dummyScreen);
        }
    }
}
