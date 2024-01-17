using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum SpawnType
{
	UNIVERSAL,
	TEAMA,
	TEAMB
}

public class SpawnPoint : NetworkBehaviour
{
    public NetworkVariable<bool> istaken = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	public SpawnType spawnType;

	private void OnTriggerEnter(Collider other)
	{
		if(IsServer && other.transform.GetComponent<PlayerNetworkHandler>())
			istaken.Value = true;
	}

	private void OnTriggerStay(Collider other)
	{
		if (IsServer && other.transform.GetComponent<PlayerNetworkHandler>())
			istaken.Value = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsServer && other.transform.GetComponent<PlayerNetworkHandler>())
			istaken.Value = false;
	}
}
