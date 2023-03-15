using FusionExamples.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resurse ce pot fi accesate global.
/// Contine liste de ScriptableObjects care reprezinta informatii despre configuratii diferite de joc
///         de exemplu: harti diferite, moduri de joc diferite
///         
/// 
/// </summary> 




public class ResourceManager : MonoBehaviour {

    // obiect ce va fi prezent in toate scenele. 
    // aici tinem toate valorile constante globale de interes in faze diferite ale jocului

    public GameType[] gameTypes;
    public MapDefinition[] mapDefinitions;
    public List<GameObject> weaponPrefabs;

    public int AfterRoundMenuScene = 6;
    public static ResourceManager Instance => Singleton<ResourceManager>.Instance;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        SetWeaponPrefabsIdentify();
    }

    // This method set an unique idetifier for every weapon prefab
    // We need this so we can be sure that we acces same weapon prefab over the network, 
    //          without the need to send prefab data
    private void SetWeaponPrefabsIdentify() {

        int i = 0;
        foreach (var weapon in weaponPrefabs) {

            weapon.GetComponent<WeaponIdentifier>()._weaponIdentifier = i++;
        }
    }
}
