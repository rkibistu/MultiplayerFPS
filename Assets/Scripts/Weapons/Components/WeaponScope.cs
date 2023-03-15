using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class WeaponScope : WeaponComponent {
    public bool Zoom => _zoom;

    [SerializeField]
    private EInputButtons _zoomButton = EInputButtons.ZoomWeapon;
    [SerializeField]
    private float _noZoomFOV = 60f;
    [SerializeField]
    private float _zoomFOV = 30f;
    [SerializeField]
    private float _zoomSpeed = 0.2f;
    [SerializeField]
    [Tooltip("Toogle false = zoom when press. Toogle true = zoom while pressed")]
    private bool _toogle = false;


    private bool _zoom = false;
    private Camera _camera;



    public override void ProcessInput(WeaponContext context, ref WeaponDesires desires, bool weaponBusy) {

        if (!HasInputAuthority)
            return;

        if (_toogle == false) {

            _zoom = (context.Input.IsSet(_zoomButton) && desires.AmmoAvailable == true);
        }
        else {

            ToggleOnInput(context, desires);
        }
    }

    public override void OnFixedUpdate(WeaponContext context, WeaponDesires desires) {

        _camera = Context.Instance.CameraContext.Camera;


        if (_zoom) {

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomFOV, _zoomSpeed);
        }
        else {

            _camera.fieldOfView = _noZoomFOV;
        }
    }


    //Handle input for zooming
    //First press -> zoom in
    //Second press -? zoom out
    private void ToggleOnInput(WeaponContext context, WeaponDesires desires) {

        if (!_zoom) {
            //no zoom + press button = zoom in
            //  return to not zoom out in the exact same frame
            _zoom = (context.PressedInput.IsSet(_zoomButton) && desires.AmmoAvailable == true);
            return;
        }

        if (_zoom) {
            // zoom  + press = zoom out
            _zoom = !(context.PressedInput.IsSet(_zoomButton) || desires.AmmoAvailable == false);
        }
        
    }
}
