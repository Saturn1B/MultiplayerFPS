using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EndHandler : NetworkBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject endHostPanel;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject looseText;

    public void End(bool win)
	{
		endPanel.SetActive(true);

		if (win)
		{
			winText.SetActive(true);
			looseText.SetActive(false);
		}
		else
		{
			winText.SetActive(false);
			looseText.SetActive(true);
		}
			

		if (IsHost)
			endHostPanel.SetActive(true);
		else
			endHostPanel.SetActive(false);
	}

	public void Quit()
	{
		DisconnectAllPlayerServerRpc();
	}

	[ServerRpc]
	private void DisconnectAllPlayerServerRpc()
	{
		PlayerNetworkHandler[] players = FindObjectsOfType<PlayerNetworkHandler>();

		foreach (PlayerNetworkHandler player in players)
		{
			if (player != GetComponentInParent<PlayerNetworkHandler>())
			{
				player.DisconnectPlayerClientRpc();
			}
		}

		GetComponentInParent<PlayerNetworkHandler>().DisconnectPlayerClientRpc();
	}
}
