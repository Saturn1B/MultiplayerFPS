using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkHandler : NetworkBehaviour
{
	[HideInInspector] public GameManager gameManager;

	public override void OnNetworkSpawn()
	{
		StartCoroutine(WaitForGM());
	}

	private IEnumerator WaitForGM()
	{
		while(gameManager == null)
		{
			gameManager = FindObjectOfType<GameManager>();
			yield return null;
		}

		switch (gameManager._currentGameMode.Value)
		{
			case 0:
				gameObject.tag = "Ally";
				break;
			case 1:
				break;
			case 2:
				break;
			case 3:
				break;
			default:
				gameObject.tag = "Ally";
				break;
		}

		if(IsOwner)
			gameManager.ConnectPlayerServerRpc();
	}
}
