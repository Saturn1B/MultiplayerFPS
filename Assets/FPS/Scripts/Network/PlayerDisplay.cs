using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour
{
	[SerializeField] private TMP_Text playerName;
	[SerializeField] private TMP_Text playerTeam;

	public void SetDisplay(string name, bool showTeam, string team)
	{
		playerName.text = name;
        if (showTeam)
        {
			if(team == "0")
            {
				playerTeam.text = "Team A";
				GetComponent<Image>().color = new Color(0, 0, 1, 0.3921569f);
            }
            else
            {
				playerTeam.text = "Team B";
				GetComponent<Image>().color = new Color(1, 0, 0, 0.3921569f);
			}
        }
		else
			playerTeam.text = "";
	}
}
