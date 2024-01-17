using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public enum RoomType
{
	STARTING,
	NORMAL,
	PORTAL,
	BOSS
}

public class Room : NetworkBehaviour
{
	[SerializeField] private RoomType roomType;
	[SerializeField] private List<GameObject> roomEnemys;

	private int playerOnRoom;
	private GameManager gameManager;
	private bool isDoorOpened;

	public override void OnNetworkSpawn()
	{
		gameManager = FindObjectOfType<GameManager>();

		foreach(GameObject enemy in roomEnemys)
		{
			enemy.GetComponentInChildren<HealthComponent>().OnDeath += OnEnemyDeathServerRpc;
		}
	}

	[ServerRpc]
	private void OnEnemyDeathServerRpc()
	{
		StartCoroutine(OnEnemyDeathHandler());
	}

	private IEnumerator OnEnemyDeathHandler()
	{
		bool isNull = false;
		while (!isNull)
		{
			if(roomEnemys.Any(element => element == null))
				isNull = true;
			yield return null;
		}
		roomEnemys.RemoveAll(GameObject => GameObject == null);
		if (roomEnemys.Count <= 0)
		{
			switch (roomType)
			{
				case RoomType.NORMAL:
					OpenDoor();
					break;
				case RoomType.PORTAL:
					ActivatePortal();
					break;
				case RoomType.BOSS:
					//END GAME
					break;
				default:
					break;
			}
		}
	}

	private void OpenDoor()
	{
		Debug.Log("Open Door");
		isDoorOpened = true;
	}

	private void ActivatePortal()
	{
		Debug.Log("Activate Portal");
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Trigger");
		if (!IsHost) return;
		Debug.Log("Trigger2");

		if (other.transform.GetComponent<PlayerNetworkHandler>() && roomType == RoomType.STARTING && !isDoorOpened)
		{
			Debug.Log("Trigger3");
			playerOnRoom++;
			if(playerOnRoom >= gameManager.maxPlayers.Value)
			{
				OpenDoor();
			}
		}
	}
}
