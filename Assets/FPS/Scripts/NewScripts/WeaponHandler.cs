using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

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

    private Vector3 recoilVector;
    [HideInInspector] public Vector3 lookAt;

    [HideInInspector] public int currentAmmo;
    private bool hasAmmo;
    private bool isReloading;
    private bool isWaiting;
    [HideInInspector] public bool isAiming;

    [HideInInspector] public UnityEvent ammoUpdate;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        cam = GetComponentInChildren<Camera>();
        normalFOV = cam.fieldOfView;
        recoilVector = Vector3.zero;
        weaponObject.transform.SetParent(sideSocket);
        weaponObject.transform.localPosition = Vector3.zero;
        SetUpNewWeapon();
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 forwardRecoil = cam.transform.forward + recoilVector;

        //Make gun rotate toward where we are looking
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, forwardRecoil * currentWeapon.shootDistance, Color.blue, 1);

        float currentDistance = currentWeapon.shootDistance;
        if (Physics.Raycast(cam.transform.position, forwardRecoil, out hit, currentWeapon.shootDistance))
            currentDistance = hit.distance;

        lookAt = cam.transform.position + forwardRecoil * currentDistance;
        weaponObject.transform.LookAt(lookAt);
        weaponObject.transform.localEulerAngles = new Vector3(weaponObject.transform.localEulerAngles.x, weaponObject.transform.localEulerAngles.y, 0.0f);

        bool isShooting = currentWeapon.isAutomatic ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);
		if (isShooting && CanShoot())
		{
            Shoot();
        }

        if(recoilVector.y > 0)
		{
            recoilVector -= new Vector3(0, currentWeapon.recoilReturnSpeed * Time.deltaTime * .1f, 0);
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
        ammoUpdate.Invoke();
        hasAmmo = true;

        weaponMesh.mesh = currentWeapon.weaponMesh;
        weaponRenderer.material = currentWeapon.weaponMat;
    }

    private void Shoot()
	{
        currentAmmo -= 1;
        ammoUpdate.Invoke();

        if (currentAmmo <= 0)
        {
            hasAmmo = false;
        }

        recoilVector += new Vector3(0, currentWeapon.recoilForce, 0) * .1f;

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

        if (hit.transform.gameObject.CompareTag(gameObject.tag)) return;

        if (hit.transform.GetComponent<HealthComponent>())
		{
            hit.transform.GetComponent<HealthComponent>().TakeDamageClientRpc(currentWeapon.bulletDamage, currentWeapon.weaponName);
        }
        else if (hit.transform.GetComponentInParent<HealthComponent>())
		{
            hit.transform.GetComponentInParent<HealthComponent>().TakeDamageClientRpc(currentWeapon.bulletDamage, currentWeapon.weaponName);
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
        ammoUpdate.Invoke();
        hasAmmo = true;
	}

    private bool CanShoot()
	{
        return hasAmmo && !isReloading && !isWaiting;
	}
}
