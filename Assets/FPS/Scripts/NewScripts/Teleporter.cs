using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Teleporter : NetworkBehaviour
{
	private GameManager gameManager;

	[SerializeField] private string newSceneName;

	[SerializeField] private GameObject effectPrefab;

	public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	private void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	public void ActivateTeleporter()
	{
		isActive.Value = true;
		GameObject e = Instantiate(effectPrefab, transform);
		e.GetComponent<NetworkObject>().Spawn();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsServer) return;

		if (other.transform.GetComponent<PlayerNetworkHandler>() && isActive.Value)
		{
			gameManager.ChangeScene(newSceneName);
		}
	}
}
