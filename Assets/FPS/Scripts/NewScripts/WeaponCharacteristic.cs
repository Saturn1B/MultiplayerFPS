using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class WeaponCharacteristic : ScriptableObject
{
	[Header("Firing Parameter")]
	[Tooltip("Number of ray casted for one shot")]
	public int bulletNumber;
	[Tooltip("Dispersion value of the shot, set to 0 for no dispersion")]
	public int dispersion;
	[Tooltip("Maximum distance for the calculation of the shot")]
	public float shootDistance;
	[Tooltip("Zoom intensity for aiming")]
	public float aimZoomIntensity;

	[Header("Weapon Parameter")]
	public string weaponName;
	[Tooltip("Damage given by each bullet shot")]
	public int bulletDamage;	
	[Tooltip("Total number of ammo in one magazine")]
	public int maxAmmoInMag;
	[Tooltip("Is the gun automatic and should keep shooting while holding the fire button")]
	public bool isAutomatic;
	[Tooltip("Time between two shot can get fired")]
	public float shootTime;
	[Tooltip("Reloading time")]
	public float reloadTime;

	[Header("Weapon Visual")]
	public float recoilForce;
	public Mesh weaponMesh;
	public Material weaponMat;
	public Animator weaponAnim;
}
