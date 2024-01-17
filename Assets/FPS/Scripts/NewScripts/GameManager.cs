using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<int> _currentGameMode = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> maxPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> connectedPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> canPlayerMove = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	public override void OnNetworkSpawn()
	{
        if (!IsHost) return;

        connectedPlayers.OnValueChanged += ConnectedPlayersCallback;
	}

	[ServerRpc]
	public void SetUpGameManagerServerRpc(string gameMode, int players)
	{
        if (!IsHost) return;

		switch (gameMode)
		{
            case "pve":
                _currentGameMode.Value = 0;
                break;
            case "control":
                _currentGameMode.Value = 1;
                break;
            case "deathmatch":
                _currentGameMode.Value = 2;
                break;
            case "mme":
                _currentGameMode.Value = 3;
                break;
            default:
                _currentGameMode.Value = 0;
                break;
        }

        maxPlayers.Value = players;

        //TO DO handle different scene rerout for all the different game mode
        NetworkManager.SceneManager.LoadScene("PhobosNetcodeScene", LoadSceneMode.Single);
    }

    void ConnectedPlayersCallback(int previous, int current)
	{
        if(current >= maxPlayers.Value)
		{
            canPlayerMove.Value = true;
		}
	}

    [ServerRpc(RequireOwnership = false)]
    public void ConnectPlayerServerRpc()
	{
        connectedPlayers.Value++;
	}
}


