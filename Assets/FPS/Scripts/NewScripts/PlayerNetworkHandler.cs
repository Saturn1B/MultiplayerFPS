using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkHandler : NetworkBehaviour
{
	[HideInInspector] public GameManager gameManager;
	private PlayerController controller;
	private WeaponHandler weaponHandler;
	private ReanimationHandler reanimationHandler;
	private HealthComponent healthComponent;

	[SerializeField] private GameObject downDetector;
	[HideInInspector] public NetworkVariable<bool> isDown = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

	public override void OnNetworkSpawn()
	{
		StartCoroutine(WaitForGM());

		NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
		controller = GetComponent<PlayerController>();
		weaponHandler = GetComponent<WeaponHandler>();
		reanimationHandler = GetComponentInChildren<ReanimationHandler>();
		healthComponent = GetComponent<HealthComponent>();

		Debug.Log("CONNECT PLAYER FIRST TIME");

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

		gameManager.gameManagerLoadedOnScene.AddListener(() =>
		{
			gameManager.ConnectPlayerServerRpc();
		});

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

		if (IsOwner)
		{
			gameManager.ConnectPlayerServerRpc();
		}

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
	}
	[ServerRpc(RequireOwnership = false)]
	public void PlayerUpServerRpc()
	{
		PlayerUpClientRpc();
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
}
