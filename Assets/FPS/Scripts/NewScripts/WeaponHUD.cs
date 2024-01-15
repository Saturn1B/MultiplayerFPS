using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private TMP_Text ammoDisplay;

    private WeaponHandler weaponHandler;

    private void Start()
    {
        weaponHandler = GetComponentInParent<WeaponHandler>();
        weaponName.text = weaponHandler.currentWeapon.weaponName;
        UpdateAmmo();
        weaponHandler.ammoUpdate.AddListener(UpdateAmmo);
    }

    private void UpdateAmmo()
    {
        ammoDisplay.text = weaponHandler.currentAmmo + " | " + weaponHandler.currentWeapon.maxAmmoInMag;
    }
}
