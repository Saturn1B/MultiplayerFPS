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
	[SerializeField] private Door door;
	[SerializeField] private Teleporter teleporter;

	[SerializeField] private Room nextRoom;

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

		if(roomType == RoomType.STARTING)
			gameManager.allPlayersLoaded.AddListener(OpenDoorServerRpc);
	}

	[ServerRpc(RequireOwnership = false)]
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
					OpenDoorServerRpc();
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

	[ServerRpc]
	private void OpenDoorServerRpc()
	{
		Debug.Log("Open Door");
		isDoorOpened = true;
		door.OpenDoor();
		nextRoom.StartRoomEnemy();
	}

	private void ActivatePortal()
	{
		Debug.Log("Activate Portal");
		teleporter.ActivateTeleporter();
	}

	//private void OnTriggerEnter(Collider other)
	//{
	//	if (!IsHost) return;

	//	if (other.transform.GetComponent<PlayerNetworkHandler>() && roomType == RoomType.STARTING && !isDoorOpened)
	//	{
	//		playerOnRoom++;
	//		if(playerOnRoom >= gameManager.maxPlayers.Value)
	//		{
	//			OpenDoorServerRpc();
	//		}
	//	}
	//}

	public void StartRoomEnemy()
    {
		if(roomEnemys.Count > 0)
        {
            foreach (var enemy in roomEnemys)
            {
				//start enemy
			}
		}
    }
}
