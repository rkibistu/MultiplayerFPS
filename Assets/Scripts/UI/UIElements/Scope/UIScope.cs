using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScope : UIBehaviour {

    [SerializeField]
    private GameObject _scopeEffect;

    public void UpdateScope(WeaponScope weaponScope) {

        if (weaponScope == null) {

            if(_scopeEffect.activeInHierarchy)
                _scopeEffect.SetActive(false);
            return;
        }

        if (_scopeEffect.activeInHierarchy && !weaponScope.Zoom) {

            _scopeEffect.SetActive(false);
            return;
        }

        if (!_scopeEffect.activeInHierarchy && weaponScope.Zoom) {

            _scopeEffect.SetActive(true);
            return;
        }
    }
}
