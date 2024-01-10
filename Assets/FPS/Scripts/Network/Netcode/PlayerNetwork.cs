using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

	[SerializeField] private Camera cam;

	Vector2 rotation = Vector2.zero;

	float sensitivity = 150;
	float yRotationLimit = 88;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		if (!IsOwner) return;

		Vector3 moveDir = Vector3.zero;

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		moveDir = transform.right * x + transform.forward * z;

		float moveSpeed = 5;
		transform.position += moveDir * moveSpeed * Time.deltaTime;

		rotation.x += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
		rotation.y += -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
		rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

		transform.localRotation = xQuat;
		cam.transform.localRotation = yQuat;
	}

	public override void OnNetworkSpawn()
	{
		cam.enabled = IsLocalPlayer;
	}
}
