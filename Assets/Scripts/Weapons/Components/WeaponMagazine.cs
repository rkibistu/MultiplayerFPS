using Fusion;
using System;
using UnityEngine;


public class WeaponMagazine : WeaponComponent {
    // PUBLIC MEMBERS

    public bool IsReloading => _isReloading;
    public float ReloadProgress => IsReloading == true ? _reloadCooldown.RemainingTime(Runner).Value / _reloadTime : 0f;
    public bool HasMagazine => _hasMagazine;
    public bool HasUnlimitedAmmo => _hasUnlimitedAmmo;
    public int MagazineAmmo => _magazineAmmo;
    public int WeaponAmmo => _weaponAmmo;

    public bool IsMainMagazine => _mainMagazine;

    // PRIVATE MEMBERS

    [SerializeField]
    private int _initialAmmo = 150;
    [SerializeField]
    private bool _hasMagazine = true;
    [SerializeField]
    private int _maxMagazineAmmo = 30;
    [SerializeField]
    private int _maxWeaponAmmo = 120;
    [SerializeField]
    private bool _hasUnlimitedAmmo;
    [SerializeField]
    private float _reloadTime = 2f;
    [Tooltip("a Weapon should have only one WeaponMagazine having this boolean set! This WeaponMagazine's ammo is showed by UI.")]
    [SerializeField]
    private bool _mainMagazine = false;

    [Networked]
    private NetworkBool _isReloading { get; set; }
    [Networked]
    private int _magazineAmmo { get; set; }
    [Networked]
    private int _weaponAmmo { get; set; }
    [Networked]
    private TickTimer _reloadCooldown { get; set; }

    private WeaponAnimation _weaponAnimation;

    // WeaponComponent INTERFACE

    public override bool IsBusy => _reloadCooldown.ExpiredOrNotRunning(Runner) == false;

    public override void ProcessInput(WeaponContext context, ref WeaponDesires desires, bool weaponBusy) {
        if (_isReloading == true)
            return;

        bool reloadRequested = context.Input.IsSet(EInputButtons.Reload) == true || _magazineAmmo == 0;

        if (weaponBusy == false && _hasMagazine == true && reloadRequested == true) {
            desires.Reload = _magazineAmmo != _maxMagazineAmmo && _weaponAmmo > 0;
        }

        int availableAmmo = _hasMagazine == true ? _magazineAmmo : _weaponAmmo;
        desires.AmmoAvailable = desires.Reload == false && availableAmmo > 0;
    }

    public override void OnFixedUpdate(WeaponContext context, WeaponDesires desires) {
        
        
        
        if (desires.HasFired == true) {
            if (_hasMagazine == true) {
                _magazineAmmo--;
            }
            else if (_hasUnlimitedAmmo == false) {
                _weaponAmmo--;
            }
        }

        if (_isReloading == true && _reloadCooldown.Expired(Runner) == true) {
            int reloadAmmo = _maxMagazineAmmo - _magazineAmmo;

            if (_hasUnlimitedAmmo == false) {
                reloadAmmo = Mathf.Min(reloadAmmo, _weaponAmmo);
                _weaponAmmo -= reloadAmmo;
            }

            _magazineAmmo += reloadAmmo;

            _isReloading = false;
        }

        if (desires.Reload == true) {
            //Start reloading
            _reloadCooldown = TickTimer.CreateFromSeconds(Runner, _reloadTime);
            _isReloading = true;

            //GetComponent<WeaponAnimation>().PlayReloadAnimation();
            PlayReloadAniamtion_Rpc();
            //announce veryoane that i am reloading
        }


       
    }

    // NetworkBehaviour INTERFACE

    public override void Spawned() {
        int initialAmmo = _hasUnlimitedAmmo == true ? int.MaxValue : _initialAmmo;

        _magazineAmmo = _hasMagazine == true ? Mathf.Clamp(initialAmmo, 0, _maxMagazineAmmo) : 0;
        _weaponAmmo = Mathf.Clamp(initialAmmo - _magazineAmmo, 0, _maxWeaponAmmo);
    }

    // PRIVATE METHODS

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void PlayReloadAniamtion_Rpc() {

        // AICI AI PUTEA SA FACI CA PENTR PROXIES, Animatia deincarcare sa fie mai scurta
        //     sa nu para niciodata ca si icnarca si trag simultan
        if (!_weaponAnimation) {
            _weaponAnimation = GetComponent<WeaponAnimation>();
        }
        _weaponAnimation.PlayReloadAnimation();
    }
}
