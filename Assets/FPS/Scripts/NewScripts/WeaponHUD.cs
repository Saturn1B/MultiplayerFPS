using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class WeaponHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private TMP_Text ammoDisplay;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasScaler canvasScaler;

    [SerializeField] private Camera cam;

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

    private void LateUpdate()
    {
        Vector2 viewportPos = cam.WorldToViewportPoint(weaponHandler.lookAt);
        rectTransform.anchoredPosition = new Vector2(0, Screen.height *  viewportPos.y);
	}
}
