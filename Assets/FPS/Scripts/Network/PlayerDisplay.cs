using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class PlayerDisplay : MonoBehaviour
{
	[SerializeField] private TMP_Text playerName;

	public void SetDisplay(string name)
	{
		playerName.text = name;
	}
}
