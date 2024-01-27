using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<int> _currentGameMode = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> maxPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> connectedPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> canPlayerMove = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> playersDown = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public UnityEvent allPlayersLoaded;
    public UnityEvent gameManagerLoadedOnScene;
    public UnityEvent gameOver;

    private bool firstConnect = true;

	public override void OnNetworkSpawn()
	{
        if (!IsHost) return;

        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
        connectedPlayers.OnValueChanged += ConnectedPlayersCallback;
        playersDown.OnValueChanged += DownPlayerCallback;
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
    }

    void ConnectedPlayersCallback(int previous, int current)
	{
        if(current >= maxPlayers.Value)
		{
            allPlayersLoaded.Invoke();
            canPlayerMove.Value = true;

            if (firstConnect)
            {
                firstConnect = false;
                switch (_currentGameMode.Value)
                {
                    case 0:
                        ChangeScene("NewPve");
                        break;
                    case 1:
                        ChangeScene("PhobosNetcodeScene");
                        break;
                    case 2:
                        ChangeScene("PhobosNetcodeScene");
                        break;
                    case 3:
                        ChangeScene("PhobosNetcodeScene");
                        break;
                    default:
                        ChangeScene("NewPve");
                        break;
                }
            }

        }
        else
        {
            canPlayerMove.Value = false;
        }
	}

    void DownPlayerCallback(int previous, int current)
	{
        if(current >= maxPlayers.Value)
		{
            gameOver.Invoke();
		}
	}

    [ServerRpc(RequireOwnership = false)]
    public void ConnectPlayerServerRpc()
	{
        connectedPlayers.Value++;
	}

    public void ChangeScene(string sceneName)
	{
        //TO DO handle different scene rerout for all the different game mode
        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadEventCompleted:
                connectedPlayers.Value = 0;
                gameManagerLoadedOnScene.Invoke();
                break;
            default:
                break;
        }
    }
}


