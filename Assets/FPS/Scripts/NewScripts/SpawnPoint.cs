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
    public NetworkVariable<int> istaken = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	public SpawnType spawnType;

	private void OnTriggerEnter(Collider other)
	{
		if(IsServer && other.transform.GetComponent<PlayerNetworkHandler>())
			istaken.Value++;
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsServer && other.transform.GetComponent<PlayerNetworkHandler>())
        {
			Debug.Log("exit spawnpoint");
			istaken.Value--;
			other.transform.GetComponent<PlayerNetworkHandler>().RemoveSpawnPointClientRpc();
		}
	}
}
