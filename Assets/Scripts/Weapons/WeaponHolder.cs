using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // obiectele setate ca target pt TwoBooneIK controller
    [Header("HandIKs that rig follows")]
    [SerializeField]
    private Transform LeftHandIK_holder;
    [SerializeField]
    private Transform RightHandIK_holder;
    [Header("Weapon rotation to follow")]
    public Transform WeaponRotation_target;

    // pozitiile mainilor, specifice pt fiecare arma
    [Header("Weapon holder positions")]
    private Transform LeftHandIK_target;
    private Transform RightHandIK_target;

    private int _targetsFound = 0;
    private int _targetsNeeded = 2;


    // AICI E LOC DE IMPROVMENETE
    // Functia asta nu art trebui apelaata decat o data cand schimbi arma. Eu momentan o apelez continuu
    public void SetInitialHandsPosition() {

        SetHandIksTarget();

        LeftHandIK_holder.position = LeftHandIK_target.position;
        LeftHandIK_holder.rotation = LeftHandIK_target.rotation;

        RightHandIK_holder.position = RightHandIK_target.position;
        RightHandIK_holder.rotation = RightHandIK_target.rotation;
    }

    private void SetHandIksTarget() {
        foreach (Transform child in this.GetComponentsInChildren<Transform>()) {

            if (child.tag == "LeftHandWeaponTarget") {
                LeftHandIK_target = child;
                _targetsFound++;
                if (_targetsFound == _targetsNeeded)
                    break;
            }
            else if (child.tag == "RightHandWeaonTarget") {
                RightHandIK_target = child;
                _targetsFound++;
                if (_targetsFound == _targetsNeeded)
                    break;
            }
        }
    }

}
