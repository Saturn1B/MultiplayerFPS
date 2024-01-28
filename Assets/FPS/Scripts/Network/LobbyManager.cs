using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private LobbyUI lobbyUI;

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;
    [HideInInspector] public string playerTeam;

    public GameObject gameManagerPrefab;
    private GameObject gameManagerInstance;

    public async void Authenticate(string name)
    {
        await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            playerName = name;
            ListLobbies();
            return;
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = name;
        Debug.Log(playerName);

        ListLobbies();
    }

    private async Task<string> CreateRelay()
	{
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(9);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private async void JoinRelay(string joinCode)
	{
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void StartGame()
	{
		try
		{
            Debug.Log("StartGame");

            string relayCode = await CreateRelay();

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
				{
                    { "StartGame", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
            });

            joinedLobby = lobby;
            lobbyUI.JoinedLobbyPanel.SetActive(false);

			if (IsLobbyHost() && IsHost)
			{
                gameManagerInstance = Instantiate(gameManagerPrefab);
                NetworkObject networkObject = gameManagerInstance.GetComponent<NetworkObject>();
                networkObject.Spawn();
                gameManagerInstance.GetComponent<GameManager>().SetUpGameManagerServerRpc(
                    joinedLobby.Data["GlobalGameMode"].Value == "pve" ? joinedLobby.Data["GlobalGameMode"].Value : joinedLobby.Data["PvPGameMode"].Value,
                    joinedLobby.MaxPlayers - joinedLobby.AvailableSlots);
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

    public bool IsLobbyHost()
	{
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
	}

	private void Update()
	{
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
	}

    private async void HandleLobbyHeartbeat()
	{
        if(hostLobby != null)
		{
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0f)
			{
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
			}
		}
	}

    private async void HandleLobbyPollForUpdates()
	{
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
                PrintPlayers();

                if(joinedLobby.Data["StartGame"].Value != "0")
				{
					if (!IsLobbyHost())
					{
                        JoinRelay(joinedLobby.Data["StartGame"].Value);
					}

                    joinedLobby = null;
                    lobbyUI.JoinedLobbyPanel.SetActive(false);

                }
            }
        }
    }

	public async void CreateLobby(string lobbyName, string globalGameMode, string pvpGameMode)
	{
		try
		{
            int maxPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
				{
                    { "GlobalGameMode", new DataObject(DataObject.VisibilityOptions.Public, globalGameMode/*, DataObject.IndexOptions.S1*/) },
                    { "PvPGameMode", new DataObject(DataObject.VisibilityOptions.Public, pvpGameMode/*, DataObject.IndexOptions.S1*/) },
                    { "StartGame", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                    //{ "Map", new DataObject(DataObject.VisibilityOptions.Public, "de_dust2")}
				}
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;

			Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);

            playerTeam = "0";

            lobbyUI.currentLobbyPlayers.text = lobby.MaxPlayers - lobby.AvailableSlots + "/" + lobby.MaxPlayers;
            lobbyUI.SetAsHost();
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
		{
            Debug.Log(e);
        }
	}

    public async void ListLobbies()
	{
        lobbyUI.ClearLobbyDisplay();
		try
		{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25, //show maximum 25 lobby
                //Filters = new List<QueryFilter>
                //{
                //    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) //show only lobby with more than one player
                //    //new QueryFilter(QueryFilter.FieldOptions.S1, "CaptureTheFlag", QueryFilter.OpOptions.EQ)
                //},
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created) //show lobby by order of creation
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
		    foreach (Lobby lobby in queryResponse.Results)
		    {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["PvPGameMode"].Value);
                lobbyUI.InstanciateLobbyDisplay(lobby);
		    }
		}
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByCode(string lobbyCode)
	{
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

            Debug.Log("Joined Lobby with code " + lobbyCode);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
	}

    public async void JoinLobby(Lobby selectedLobby)
    {
        try
        {
            if(selectedLobby.Data["GlobalGameMode"].Value == "pvp"
                && (selectedLobby.Data["PvPGameMode"].Value == "control" || selectedLobby.Data["PvPGameMode"].Value == "mme"))
            {
                playerTeam = ((selectedLobby.MaxPlayers - selectedLobby.AvailableSlots) % 2).ToString();
            }

            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer(playerTeam)
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(selectedLobby.Id, joinLobbyByIdOptions);
            joinedLobby = lobby;

            //Debug.Log("Joined Lobby with code " + lobbyCode);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void QuickJoin()
	{
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer(string team = "0")
	{
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                        {"PlayerTeam", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, team) },

                    }
        };
    }

    private void PrintPlayers()
	{
        PrintPlayers(joinedLobby);
	}
    private void PrintPlayers(Lobby lobby)
	{
        lobbyUI.ClearPlayerDisplay();
        Debug.Log("Players in Lobby " + lobby.Name + " " + lobby.Data["PvPGameMode"].Value/* + " " + lobby.Data["Map"].Value*/);
        lobbyUI.currentLobbyPlayers.text = lobby.MaxPlayers - lobby.AvailableSlots + "/" + lobby.MaxPlayers;

        bool showTeam = false;
        if (lobby.Data["GlobalGameMode"].Value == "pvp"
            && (lobby.Data["PvPGameMode"].Value == "control" || lobby.Data["PvPGameMode"].Value == "mme"))
        {
            showTeam = true;
        }

        List<Player> playerA = new List<Player>();
        List<Player> playerB = new List<Player>();

        foreach (Player player in lobby.Players)
		{
            if (player.Data["PlayerTeam"].Value == "0")
                playerA.Add(player);
            else
                playerB.Add(player);
        }
        foreach (Player player in playerA)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
            lobbyUI.InstanciatePlayerDisplay(player, showTeam);
        }
        foreach (Player player in playerB)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
            lobbyUI.InstanciatePlayerDisplay(player, showTeam);
        }
    }

    private async void UpdateLobbyGameMode(string gameMode)
	{
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "PvPGameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                }
            });
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                        {
                            {"Player Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                        }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
	{
        try
        {
            if(joinedLobby.MaxPlayers - joinedLobby.AvailableSlots <= 1)
			{
                DeleteLobby();
                Debug.Log("DeletLobby");
			}
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
            hostLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void KickPlayer()
	{
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void MigrateHost()
	{
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void DeleteLobby()
	{
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
