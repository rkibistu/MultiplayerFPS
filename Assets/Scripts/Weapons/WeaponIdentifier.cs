using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIdentifier : MonoBehaviour
{
    public int _weaponIdentifier;

    public static int Compare(GameObject x, GameObject y) {
        return x.GetComponent<WeaponIdentifier>()._weaponIdentifier.CompareTo(y.GetComponent<WeaponIdentifier>()._weaponIdentifier);
    }
}
