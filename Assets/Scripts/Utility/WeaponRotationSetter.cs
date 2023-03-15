using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotationSetter : MonoBehaviour {

    [SerializeField]
    [Tooltip("Pozitia modelului + mainilor in lume")]
    private Transform _position;
    [SerializeField]
    [Tooltip("pozitia dorita pentru zoom")]
    private Transform _zoomWeaponPosition;
    [Tooltip("pozitia default a armei")]
    [SerializeField]
    private Transform _defaultPosition;
    
    private Transform _target;

    private Transform _parent;

    private void OnEnable() {

        //_target = null - ar putea fi apelat doar cand o arma este ridicata de pe jos.
        //              acum se apeleaza asta si cand scimbia rmele intre ele -> not optimal
        _target = null;
    }


    private void FindAimTarget() {
        
        _parent = transform.parent;
        if (_parent == null)
            return;

        while (_parent.GetComponent<WeaponHolder>() == null) {

            Debug.Log(_parent == null);
            _parent = _parent.parent;
            if (_parent == null) {
                Debug.LogError("this should not be null. Missing WeaponHolder from parent");
                return;
            }
        }

        _target = _parent.GetComponent<WeaponHolder>().WeaponRotation_target;
    }

    // Update is called once per frame
    void Update() {
        if (_target != null) {

            transform.rotation = _target.rotation;
        }
        else {

            FindAimTarget();
        }

        
    }


    // Change weapon position to look like we aim
    private void TestZoomPositionOfWeapon() {

        if (Input.GetKeyDown(KeyCode.K)) {

            GetComponent<TransformTranslator>().LerpTo(_position, _zoomWeaponPosition.localPosition, 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.L)) {

            GetComponent<TransformTranslator>().MoveTo(_position, _defaultPosition.localPosition);
        }
    }
}
