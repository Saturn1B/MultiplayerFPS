using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameOverHandler : NetworkBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverHostPanel;

    public void GameOver()
	{
		gameOverPanel.SetActive(true);

		if (IsHost)
			gameOverHostPanel.SetActive(true);
		else
			gameOverHostPanel.SetActive(false);
	}

	public void DeactivateGameOver()
	{
		gameOverPanel.SetActive(false);
	}

	public void Quit()
	{
		DisconnectAllPlayerServerRpc();
	}

	public void Restart()
	{
		ResetAllPlayerServerRpc();
	}

	[ServerRpc]
	private void ResetAllPlayerServerRpc()
	{
		PlayerNetworkHandler[] players = FindObjectsOfType<PlayerNetworkHandler>();
		GameManager gameManager = FindObjectOfType<GameManager>();

		gameManager.ChangeScene("NewPve");

		foreach (PlayerNetworkHandler player in players)
		{
			player.RestartPlayerClientRpc();
		}
	}

	[ServerRpc]
	private void DisconnectAllPlayerServerRpc()
	{
		PlayerNetworkHandler[] players = FindObjectsOfType<PlayerNetworkHandler>();

		foreach (PlayerNetworkHandler player in players)
		{
			if(player != GetComponentInParent<PlayerNetworkHandler>())
			{
				player.DisconnectPlayerClientRpc();
			}
		}

		GetComponentInParent<PlayerNetworkHandler>().DisconnectPlayerClientRpc();
	}
}
