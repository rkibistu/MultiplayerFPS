using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// Seteaza dropdownul pentru regiune din menu-ul InfoSetupScreen
/// 
/// La schimbarea regiunii de client -> actualizeaza si PhotonSettings
/// 
/// </summary>
public class RegionUI : MonoBehaviour {
    private void Awake() {
        if (TryGetComponent(out TMP_Dropdown dropdown)) {
            // TODO: update options once we can request a list of regions
            string[] options = new string[] { "us", "eu", "asia" };

            dropdown.AddOptions(new List<string>(options));
            dropdown.onValueChanged.AddListener((index) => {
                string region = dropdown.options[index].text;
                Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion = region;
                Debug.Log($"Setting region to {region}");
            });

            string curRegion = Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion;
            Debug.Log($"Initial region is {curRegion}");
            int curIndex = dropdown.options.FindIndex((op) => op.text == curRegion);
            dropdown.value = curIndex != -1 ? curIndex : 0;
        }
    }
}