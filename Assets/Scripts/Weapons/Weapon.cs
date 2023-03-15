using Fusion;
using UnityEngine;


public struct WeaponDesires {
    public bool AmmoAvailable;
    public bool Fire;
    public bool HasFired;
    public bool Reload;
    public bool Zoom;
}

public class Weapon : NetworkBehaviour {
    // PUBLIC MEMBERS

    public int WeaponSlot => _weaponSlot;
    public bool IsArmed { get; private set; }

    public Transform[] BarrelTransforms { get; private set; }
    public WeaponAction[] WeaponActions => _weaponActions;
    public WeaponMagazine WeaponMagazine => _mainMagazine;
    public WeaponScope WeaponScope => _weaponScope;
    public GameObject PickableWeaponPrefab => _pickableWeaponPrefab;

    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public GameObject Magazine;

    // PRIVATE MEMBERS

    [SerializeField]
    private int _weaponSlot;

    [Header("UI")]
    [SerializeField]
    private string _displayName;
    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private GameObject _pickableWeaponPrefab;


    private WeaponAction[] _weaponActions;

    private WeaponMagazine _mainMagazine;

    private WeaponScope _weaponScope;

    private WeaponAnimation _weaponAnimation;


    // PUBLIC METHODS

    public void ArmWeapon() {
        if (IsArmed == true)
            return;

        IsArmed = true;
        OnArmed();
    }

    public void DisarmWeapon() {
        if (IsArmed == false)
            return;

        IsArmed = false;
        OnDisarmed();
    }

    public virtual void ProcessInput(WeaponContext context) {
        Assert.Check(Runner.Stage != default, "Process input should be called from FixedUpdateNetwork");

        // When weapon is busy (e.g. firing, reloading) we cannot start new actions
        bool isBusy = IsBusy();

        for (int i = 0; i < _weaponActions.Length; i++) {
            if (i > 0 && isBusy == false) {
                // Check busy status of previous weapon action
                // because it might changed this tick
                isBusy |= _weaponActions[i - 1].IsBusy();
            }

            _weaponActions[i].ProcessInput(context, isBusy);
        }
    }

    public virtual void OnRender(WeaponContext context) {
        for (int i = 0; i < _weaponActions.Length; i++) {
            _weaponActions[i].OnRender(context);
        }
    }

    public bool IsBusy() {
        for (int i = 0; i < _weaponActions.Length; i++) {
            if (_weaponActions[i].IsBusy() == true)
                return true;
        }

        return false;
    }

    // PROTECTED METHODS

    protected virtual void OnArmed() {
        // Do visual effects, sounds here
        // OnArmed is executed in render only

        _weaponAnimation.StopAllAniamtions();
    }

    protected virtual void OnDisarmed() {
        // OnDisarmed is executed in render only
    }

    // MONOBEHAVIOUR

    protected virtual void Awake() {
        _weaponActions = GetComponentsInChildren<WeaponAction>(false);

        if (_weaponActions.Length > 0) {
            BarrelTransforms = new Transform[_weaponActions.Length];

            for (int i = 0; i < _weaponActions.Length; i++) {
                _weaponActions[i].Initialize(this, (byte)i);

                BarrelTransforms[i] = _weaponActions[i].BarrelTransform;
            }

            GetComponentsInChild();
        }
        else {
            // Make sure there is at least one dummy barrel transform
            BarrelTransforms = new Transform[] { transform };
        }
    }

    // PRIVATE METHODS

    private void GetMainMagazine(GameObject obj) {


        WeaponMagazine childMagazine = obj.GetComponent<WeaponMagazine>();

        if (childMagazine != null && childMagazine.IsMainMagazine) {

            if (_mainMagazine != null) {
                Debug.LogError("A Weapon should have only one mainMagazine!");
            }
            _mainMagazine = childMagazine;
        }



    }
    private void GetWeaponScope(GameObject obj) {


        WeaponScope childScope = obj.GetComponent<WeaponScope>();

        if (childScope != null) {

            if (_weaponScope != null) {
                Debug.LogError("A Weapon should have only one WeaponScope!");
            }
            _weaponScope = childScope;
        }

    }
    private void GetWeaponAnimation(GameObject obj) {

        WeaponAnimation childScope = obj.GetComponent<WeaponAnimation>();

        if (childScope != null) {

            if (_weaponAnimation != null) {
                Debug.LogError("A Weapon should have only one weaponAnimation class aatched!");
            }
            _weaponAnimation = childScope;
        }
    }

    private void GetComponentsInChild() {


        foreach (Transform child in transform) {

            GetWeaponScope(child.gameObject);
            GetMainMagazine(child.gameObject);
            GetWeaponAnimation(child.gameObject);
        }
        if (_mainMagazine == null)
            Debug.LogError("a Weapon needs at least one mainMagazine!");

    }






}
