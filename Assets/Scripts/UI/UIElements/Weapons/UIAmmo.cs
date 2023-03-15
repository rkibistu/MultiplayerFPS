using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAmmo : UIBehaviour
{
    // PRIVATE METHODS

    [SerializeField]
    private UIValue _magazineAmmoValue;
    [SerializeField]
    private UIValue _weaponAmmoValue;
    [SerializeField]
    private Image _weaponImage;

    // PUBLIC EMTHODS

    public void UpdateAmmo(WeaponMagazine weaponMagazine) {

        if (weaponMagazine == null)
            return;

        _magazineAmmoValue.SetValue(weaponMagazine.MagazineAmmo);
        _weaponAmmoValue.SetValue(weaponMagazine.WeaponAmmo);
    }
    public void UpdateWeaponImage(Weapon weapon) {

        _weaponImage.sprite = weapon.Icon;
    }
    
}
