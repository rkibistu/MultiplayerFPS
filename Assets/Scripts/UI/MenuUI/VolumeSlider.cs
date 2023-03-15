using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Seteaza volumul initial din UnityPrefs
/// 
/// La modificarea volumului atualizeaza setarile din untiy pt voolum
/// 
/// </summary>

public class VolumeSlider : MonoBehaviour {
    public string mixerParameter;
    public string mixerGroup;
    private float lastVal;

    //SET INITIAL VaLUE FROM UNITY PREFS (utlima val setata de client)

    private void OnEnable() {
        if (TryGetComponent(out Slider slider)) {
            lastVal = slider.value = PlayerPrefs.GetFloat(mixerParameter, 0.75f);
            slider.onValueChanged.AddListener((val) => {
                if (Mathf.Round(val * 10) != Mathf.Round(lastVal * 10)) {
                    AudioManager.Play("hoverUI", mixerGroup);
                    lastVal = val;
                }
            });
        }
    }
}
