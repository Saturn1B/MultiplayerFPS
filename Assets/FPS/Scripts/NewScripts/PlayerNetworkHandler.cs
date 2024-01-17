using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkHandler : NetworkBehaviour
{
	[HideInInspector] public GameManager gameManager;
	private PlayerController controller;

	public override void OnNetworkSpawn()
	{
		StartCoroutine(WaitForGM());
		NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
		controller = GetComponent<PlayerController>();
	}

	private void Start()
	{
		if (IsOwner)
			ChooseSpawn();
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
}
