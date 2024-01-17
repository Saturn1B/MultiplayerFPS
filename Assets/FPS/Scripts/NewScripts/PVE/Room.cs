using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum RoomType
{
	STARTING,
	NORMAL,
	DOOR,
	BOSS
}

public class Room : NetworkBehaviour
{
	[SerializeField] private RoomType roomType;
	[SerializeField] private List<GameObject> roomEnemys;
}
