using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponHandler : NetworkBehaviour
{
    public WeaponCharacteristic currentWeapon;

    [SerializeField] private MeshFilter weaponMesh;
    [SerializeField] private MeshRenderer weaponRenderer;
    [SerializeField] private Animator weaponAnim;

    [SerializeField] private GameObject weaponObject;
    [SerializeField] private Transform sideSocket;
    [SerializeField] private Transform aimSocket;

    [SerializeField] private Transform muzzlePoint;
    private Camera cam;
    private float normalFOV;

    private int currentAmmo;
    private bool hasAmmo;
    private bool isReloading;
    private bool isWaiting;
    private bool isAiming;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        cam = GetComponentInChildren<Camera>();
        normalFOV = cam.fieldOfView;
        weaponObject.transform.SetParent(sideSocket);
        weaponObject.transform.localPosition = Vector3.zero;
        SetUpNewWeapon();
    }

    private void Update()
    {
        if (!IsOwner) return;

        //Make gun rotate toward where we are looking
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, cam.transform.forward * currentWeapon.shootDistance, Color.red, 1);
        Vector3 lookAt = cam.transform.position + cam.transform.forward * currentWeapon.shootDistance;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.shootDistance))
            lookAt = cam.transform.position + cam.transform.forward * hit.distance;
        weaponObject.transform.LookAt(lookAt);
        weaponObject.transform.localEulerAngles = new Vector3(weaponObject.transform.localEulerAngles.x, weaponObject.transform.localEulerAngles.y, 0.0f);


        if (currentWeapon.isAutomatic)
		{
			if (Input.GetKey(KeyCode.Mouse0) && CanShoot())
			{
                Shoot();
            }
        }
        else
		{
			if (Input.GetKeyDown(KeyCode.Mouse0) && CanShoot())
			{
                Shoot();
			}
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
            StartCoroutine(Reload());
		}

		if (Input.GetKeyDown(KeyCode.Mouse1) && !isAiming)
		{
            Aim();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && isAiming)
        {
            Aim();
        }
    }

    private void SetUpNewWeapon()
	{
        currentAmmo = currentWeapon.maxAmmoInMag;
        hasAmmo = true;

        weaponMesh.mesh = currentWeapon.weaponMesh;

        weaponRenderer.material = currentWeapon.weaponMat;
    }

    private void Shoot()
	{
        currentAmmo -= 1;

        if (currentAmmo <= 0)
        {
            hasAmmo = false;
        }

        StartCoroutine(WaitShootTime());

        ShootServerRpc();
    }

    [ServerRpc]
    private void ShootServerRpc()
	{
        Debug.Log("shoot: " + OwnerClientId);

        for (int i = 0; i < currentWeapon.bulletNumber; i++)
        {
            Vector3 direction = Quaternion.Euler(Random.Range(-currentWeapon.dispersion, currentWeapon.dispersion) * Mathf.Cos(Random.Range(0, 2 * Mathf.PI)),
                Random.Range(-currentWeapon.dispersion, currentWeapon.dispersion) * Mathf.Sin(Random.Range(0, 2 * Mathf.PI)), 0) * weaponObject.transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(muzzlePoint.position, direction, out hit, currentWeapon.shootDistance))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                Hit(hit);
            }
            else
            {
                Debug.Log("Missed!");
            }

            Debug.DrawRay(muzzlePoint.position, direction * currentWeapon.shootDistance, Color.red, 3f);
        }
    }

    private void Aim()
	{
        isAiming = !isAiming;
        if (isAiming)
		{
            cam.fieldOfView -= currentWeapon.aimZoomIntensity;
            weaponObject.transform.SetParent(aimSocket);
            weaponObject.transform.localPosition = Vector3.zero;
        }
        else
		{
            cam.fieldOfView = normalFOV;
            weaponObject.transform.SetParent(sideSocket);
            weaponObject.transform.localPosition = Vector3.zero;
        }
    }

    private IEnumerator WaitShootTime()
	{
        isWaiting = true;
        yield return new WaitForSeconds(currentWeapon.shootTime);
        isWaiting = false;
	}

    private void Hit(RaycastHit hit)
	{
        Debug.Log("hit: " + OwnerClientId);

        if (hit.transform.GetComponent<HealthComponent>())
		{
            hit.transform.GetComponent<HealthComponent>().TakeDamageClientRpc(currentWeapon.bulletDamage, currentWeapon.weaponName);
        }
    }

    private IEnumerator Reload()
	{
        isReloading = true;
        if (isAiming) Aim();
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        if (Input.GetKey(KeyCode.Mouse1) && !isAiming) Aim();
        isReloading = false;
        currentAmmo = currentWeapon.maxAmmoInMag;
        hasAmmo = true;
	}

    private bool CanShoot()
	{
        return hasAmmo && !isReloading && !isWaiting;
	}
}
