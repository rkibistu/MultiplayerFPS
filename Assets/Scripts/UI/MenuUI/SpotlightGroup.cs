using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Folosim clasa asta cand un obiect are mai multi copii, dar vrem ca doar unul din ei sa fie activ la un moment dat.
/// Atasam clasa aceasta de componenta parinte care contine acei copii.
///     clasa SpotlightBridge este un exemplu de cum putem modfica ce copil este activ 
/// </summary>

public class SpotlightGroup : MonoBehaviour {
    
   

    // dictionar cu toate grupurile create si numele dupa care acesta pot fi gasite
    //  folosit pentru a putea modifica focusul din oricare grup existent
    private static readonly Dictionary<string, SpotlightGroup> spotlights = new Dictionary<string, SpotlightGroup>();

    public string searchName = ""; // numele dupa care poate fi gasit acest gameObject folosind metoda Search implementata mai jos
    public int defaultIndex = -1; // indexul in lsta al copilului ce vrem sa fie activ
    public List<GameObject> objects; // lista de copii ce vrem sa fie activi ( pe rand)

    private GameObject focused = null;

    public static bool Search(string spotlightName, out SpotlightGroup spotlight) {
        return spotlights.TryGetValue(spotlightName, out spotlight);
    }

    private void OnEnable() {
        if (string.IsNullOrEmpty(searchName) == false) {
            spotlights.Add(searchName, this);
        }
    }

    private void OnDisable() {
        if (string.IsNullOrEmpty(searchName) == false) {
            spotlights.Remove(searchName);
        }
    }

    private void Awake() {
        objects.ForEach((obj) => obj.SetActive(false));
        if (defaultIndex != -1) {
            FocusIndex(defaultIndex);
        }
    }

    public void FocusIndex(int index) {
        if (focused) focused.SetActive(false);
        focused = objects[index];
        focused.SetActive(true);
    }

    public void Defocus() {
        if (focused) focused.SetActive(false);
        focused = null;
    }
}
