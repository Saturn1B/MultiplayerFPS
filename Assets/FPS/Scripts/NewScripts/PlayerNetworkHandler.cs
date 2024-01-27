using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerNetworkHandler : NetworkBehaviour
{
	[HideInInspector] public GameManager gameManager;
	private PlayerController controller;
	private WeaponHandler weaponHandler;
	private ReanimationHandler reanimationHandler;
	private HealthComponent healthComponent;
	private GameOverHandler gameOverHandler;

	[SerializeField] private GameObject downDetector;
	[HideInInspector] public NetworkVariable<bool> isDown = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

	private NetworkVariable<int> playerTeam = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

	public override void OnNetworkSpawn()
	{
		StartCoroutine(WaitForGM());

		NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
		controller = GetComponent<PlayerController>();
		weaponHandler = GetComponent<WeaponHandler>();
		reanimationHandler = GetComponentInChildren<ReanimationHandler>();
		healthComponent = GetComponent<HealthComponent>();
		gameOverHandler = GetComponentInChildren<GameOverHandler>();

		Debug.Log("CONNECT PLAYER FIRST TIME");

		playerTeam.OnValueChanged += ((int old, int current) =>
		{
			switch (gameManager._currentGameMode.Value)
			{
				case 0:
					gameObject.tag = "Ally";
					break;
				case 1:
					gameObject.tag = current == 0 ? "TeamA" : "TeamB";
					break;
				case 2:
					break;
				case 3:
					gameObject.tag = current == 0 ? "TeamA" : "TeamB";
					break;
				default:
					gameObject.tag = "Ally";
					break;
			}
		});

		if (IsOwner)
            ChooseSpawn();
    }

	private void Start()
	{
		//if (IsOwner)
			//ChooseSpawn();
	}

	private IEnumerator WaitForGM()
	{
		while(gameManager == null)
		{
			gameManager = FindObjectOfType<GameManager>();
			yield return null;
		}

		gameManager.gameOver.AddListener(() =>
		{
			HandleGameOverClientRpc();
		});

		gameManager.gameManagerLoadedOnScene.AddListener(() =>
		{
			gameManager.ConnectPlayerServerRpc();
		});

        if (IsOwner)
        {
			LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();

			playerTeam.Value = lobbyManager.playerTeam == "0" ? 0 : 1;
        }

		switch (gameManager._currentGameMode.Value)
		{
			case 0:
				gameObject.tag = "Ally";
				break;
			case 1:
				gameObject.tag = playerTeam.Value == 0 ? "TeamA" : "TeamB";
				break;
			case 2:
				break;
			case 3:
				gameObject.tag = playerTeam.Value == 0 ? "TeamA" : "TeamB";
				break;
			default:
				gameObject.tag = "Ally";
				break;
		}

		if (IsOwner)
		{
			gameManager.ConnectPlayerServerRpc();
		}

	}

	[ClientRpc]
	private void HandleGameOverClientRpc()
	{
		if (!IsOwner) return;

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		controller.isGameOver = true;
		gameOverHandler.GameOver();
	}

	private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
	{
		switch (sceneEvent.SceneEventType)
		{
			case SceneEventType.LoadEventCompleted:
				Debug.Log("NEW SCENE LOADED");
				if (IsOwner) ChooseSpawn();
				break;
			default:
				break;
		}
	}

	public void ChooseSpawn()
	{
		if (!IsOwner) return;

		SpawnPoint[] spawnpoints = FindObjectsOfType<SpawnPoint>();
		SpawnPoint choosenSpawn = null;
		foreach (SpawnPoint spawn in spawnpoints)
		{
			if (!spawn.istaken.Value)
			{
				choosenSpawn = spawn;
			}
		}
		if (choosenSpawn != null)
		{
			controller.Teleport(choosenSpawn.transform.position);
		}
	}

	public void PlayerDown()
	{
		isDown.Value = true;
		controller.PlayerDown();
		weaponHandler.PlayerDown();
		downDetector.SetActive(true);
		PlayerDownServerRpc();
	}
	[ServerRpc]
	void PlayerDownServerRpc()
	{
		gameManager.playersDown.Value++;
	}

	[ServerRpc(RequireOwnership = false)]
	public void PlayerUpServerRpc()
	{
		PlayerUpClientRpc();

		if(gameManager.playersDown.Value > 0)
			gameManager.playersDown.Value--;
	}
	[ClientRpc]
	public void PlayerUpClientRpc()
	{
		if (!IsOwner) return;

		isDown.Value = false;
		controller.PlayerUp();
		weaponHandler.PlayerUp();
		downDetector.SetActive(false);

		healthComponent.currentHealth.Value = healthComponent.maxHealth;
	}

	void OnTriggerEnter(Collider other)
	{
		if (!IsOwner) return;

		Debug.Log("detect: " + other.gameObject.name);

		if (other.transform.parent && other.transform.parent.gameObject == this.gameObject) return;

		if (other.isTrigger && other.transform.GetComponentInParent<PlayerNetworkHandler>() && other.transform.GetComponentInParent<PlayerNetworkHandler>().isDown.Value)
		{
			Debug.Log("detect down player");
			reanimationHandler.ActivateReaUi();
			reanimationHandler.reaPlayers.AddListener(other.transform.GetComponentInParent<PlayerNetworkHandler>().PlayerUpServerRpc);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!IsOwner) return;

		if (other.isTrigger && other.transform.GetComponentInParent<PlayerNetworkHandler>())
		{
			reanimationHandler.DeactivateReaUi();
			reanimationHandler.reaPlayers.RemoveAllListeners();
		}
	}

	[ClientRpc]
	public void RestartPlayerClientRpc()
	{
		PlayerUpServerRpc();
		weaponHandler.ReloadClientRpc();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		controller.isGameOver = false;
		gameOverHandler.DeactivateGameOver();
	}

	[ClientRpc]
	public void DisconnectPlayerClientRpc()
	{
		NetworkManager.Singleton.Shutdown();
		Cleanup();
		SceneManager.LoadScene("PhobosTestScene", LoadSceneMode.Single);
	}

	private void Cleanup()
	{
		if (NetworkManager.Singleton != null)
		{
			Destroy(NetworkManager.Singleton.gameObject);
		}
	}
}
