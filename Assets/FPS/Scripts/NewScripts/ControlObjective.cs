using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ControlObjective : NetworkBehaviour
{
    public NetworkVariable<int> teamA = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> teamB = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private NetworkVariable<int> controllingTeam = new NetworkVariable<int>(2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	GameManager gameManager;

    public override void OnNetworkSpawn()
    {
		if (!IsServer) return;

		gameManager = FindObjectOfType<GameManager>();
		teamA.OnValueChanged += (int old, int current) =>
		{
			if (current > teamB.Value)
			{
				controllingTeam.Value = 0;
			}
			else if(current == teamB.Value)
            {
				controllingTeam.Value = 2;
            }
			else if(current < teamB.Value)
            {
				controllingTeam.Value = 1;
            }
		};
		teamB.OnValueChanged += (int old, int current) =>
		{
			if (current > teamA.Value)
			{
				controllingTeam.Value = 1;
			}
			else if (current == teamA.Value)
			{
				controllingTeam.Value = 2;
			}
			else if (current < teamA.Value)
			{
				controllingTeam.Value = 0;
			}
		};
		controllingTeam.OnValueChanged += (int old, int current) =>
		{
			if (old == current) return;

			StopAllCoroutines();

			if (current != 2)
            {
				StartCoroutine(CalculateTeamScores(current));
            }
		};
	}

    private void OnTriggerEnter(Collider other)
	{
		if (IsServer && other.transform.GetComponent<PlayerNetworkHandler>())
        {
            if (other.transform.CompareTag("TeamA"))
            {
				teamA.Value++;
			}
			else if (other.transform.CompareTag("TeamB"))
            {
				teamB.Value++;
			}
			other.transform.GetComponent<PlayerNetworkHandler>().AddObjectiveClientRpc();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsServer && other.transform.GetComponent<PlayerNetworkHandler>() && other.transform.GetComponent<PlayerNetworkHandler>().isInObjective.Value)
		{
			if (other.transform.CompareTag("TeamA"))
			{
				teamA.Value--;
			}
			else if (other.transform.CompareTag("TeamB"))
			{
				teamB.Value--;
			}
			other.transform.GetComponent<PlayerNetworkHandler>().RemoveObjectiveClientRpc();
		}
	}

	public IEnumerator CalculateTeamScores(int team)
    {
		if(team == 0)
        {
			gameManager.scoreTeamA.Value++;
        }
		else if (team == 1)
        {
			gameManager.scoreTeamB.Value++;
        }
		yield return new WaitForSeconds(1f);

		StartCoroutine(CalculateTeamScores(team));
    }

	[ServerRpc(RequireOwnership = false)]
	public void RemoveFromTeamServerRpc(int team)
    {
		if (team == 0)
			teamA.Value--;
		else if (team == 1)
			teamB.Value--;
	}
}
