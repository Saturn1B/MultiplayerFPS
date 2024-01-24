using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Teleporter : NetworkBehaviour
{
	private GameManager gameManager;

	[SerializeField] private string newSceneName;
	[SerializeField] private GameObject teleporterObject;

	private void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	public void ActivateTeleporter()
	{
		teleporterObject.SetActive(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsServer) return;

		if (other.transform.GetComponent<PlayerNetworkHandler>())
		{
			gameManager.ChangeScene(newSceneName);
		}
	}
}
