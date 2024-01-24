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

	[HideInInspector] public string globalGameMode;
	[HideInInspector] public string pvpGameMode;
	[HideInInspector] public string nextTeam;

	public void SetDisplay(string name, int totalPlayer, int connectedPlayer, string globalGameMode, string pvpGameMode, Lobby lobby)
	{
		this.globalGameMode = globalGameMode;
		this.pvpGameMode = pvpGameMode;
		nextTeam = connectedPlayer%2 == 0 ? "0" : "1";

		lobbyName.text = name;
		lobbyPlayers.text = connectedPlayer + "/" + totalPlayer;
		lobbyGameMode.text = pvpGameMode == "" ? globalGameMode : globalGameMode + " | " + pvpGameMode;
		selectedLobby = lobby;

		joinButton.onClick.AddListener(() =>
		{
			lobbyUI.SwitchLobbyPanel(globalGameMode, pvpGameMode);
			Join();
		});
	}

	void Join()
	{
		if (globalGameMode == "pvp" && (pvpGameMode == "control" || pvpGameMode == "mme"))
		{
			lobbyUI.currentTeamLobbyName.text = lobbyName.text;
			lobbyUI.currentTeamLobbyPlayers.text = lobbyPlayers.text;
			lobbyUI.currentTeamLobbyGameMode.text = lobbyGameMode.text;
		}
		else
		{
			lobbyUI.currentLobbyName.text = lobbyName.text;
			lobbyUI.currentLobbyPlayers.text = lobbyPlayers.text;
			lobbyUI.currentLobbyGameMode.text = lobbyGameMode.text;
		}

		lobbyManager.JoinLobby(selectedLobby.Id, globalGameMode, pvpGameMode, nextTeam);
	}
}
