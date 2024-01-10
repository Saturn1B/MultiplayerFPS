using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

public class LobbyDisplay : MonoBehaviour
{
	[SerializeField] private TMP_Text lobbyName;
	[SerializeField] private TMP_Text lobbyPlayers;
	[SerializeField] private TMP_Text lobbyGameMode;
	[SerializeField] private Button joinButton;


	Lobby selectedLobby;
	[HideInInspector] public LobbyManager lobbyManager;
	[HideInInspector] public LobbyUI lobbyUI;

	public void SetDisplay(string name, int totalPlayer, int connectedPlayer, string globalGameMode, string pvpGameMode, Lobby lobby)
	{
		lobbyName.text = name;
		lobbyPlayers.text = connectedPlayer + "/" + totalPlayer;
		lobbyGameMode.text = pvpGameMode == "" ? globalGameMode : globalGameMode + " | " + pvpGameMode;
		selectedLobby = lobby;

		joinButton.onClick.AddListener(() =>
		{
			lobbyUI.SwitchLobbyPanel();
			Join();
		});
	}

	void Join()
	{
		lobbyUI.currentLobbyName.text = lobbyName.text;
		lobbyUI.currentLobbyPlayers.text = lobbyPlayers.text;
		lobbyUI.currentLobbyGameMode.text = lobbyGameMode.text;
		lobbyManager.JoinLobby(selectedLobby.Id);
	}
}
