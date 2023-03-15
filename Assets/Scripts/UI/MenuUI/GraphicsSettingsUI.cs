using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// 
/// Folosit in cadrul menu-ului de setare a calitatii graficii: GraphicsOptionScreen
/// 
/// Modifica setarile de grafica din unity in cazul in care valorile sunt modificare prin itnermediul menu-ului.
///     Functiile sunt elgate de butoanele din menu
/// 
/// </summary>
public class GraphicsSettingsUI : MonoBehaviour {
    public TMP_Dropdown graphicsDropdown;

    private void Awake() {
        InitGraphicsDropdown();
    }

    public void InitGraphicsDropdown() {
        string[] names = QualitySettings.names;
        List<string> options = new List<string>();

        for (int i = 0; i < names.Length; i++) {
            options.Add(names[i]);
        }
        graphicsDropdown.AddOptions(options);
        QualitySettings.SetQualityLevel(graphicsDropdown.options.Count - 1);
        graphicsDropdown.value = graphicsDropdown.options.Count - 1;
    }

    public void SetGraphicsQuality() {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }
}
