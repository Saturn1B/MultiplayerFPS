using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Door : NetworkBehaviour
{
	[SerializeField] private GameObject gate;

	public void OpenDoor()
	{
		//gate.SetActive(false);
		gate.GetComponent<NetworkObject>().Despawn();
	}
}
