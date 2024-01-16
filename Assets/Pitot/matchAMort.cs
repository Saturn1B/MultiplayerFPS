using UnityEngine;
using System.Collections.Generic;

public class matchAMort : MonoBehaviour
{
    public GameObject redPlayerPrefab; // Pr�fab du joueur de l'�quipe rouge
    public GameObject bluePlayerPrefab; // Pr�fab du joueur de l'�quipe bleue

    public List<Transform> redTeamSpawnPoints;
    public List<Transform> blueTeamSpawnPoints;

    public int numberOfPlayersPerTeam = 5; // Nombre de joueurs par �quipe

    void Start()
    {
        InstantiatePlayers();
    }

    void InstantiatePlayers()
    {
        // Instantiate les joueurs pour l'�quipe rouge avec une rotation de 180 degr�s
        InstantiateTeamPlayers(redTeamSpawnPoints, redPlayerPrefab, Quaternion.Euler(0f, 180f, 0f), numberOfPlayersPerTeam);

        // Instantiate les joueurs pour l'�quipe bleue
        InstantiateTeamPlayers(blueTeamSpawnPoints, bluePlayerPrefab, Quaternion.identity, numberOfPlayersPerTeam);
    }

    void InstantiateTeamPlayers(List<Transform> spawnPoints, GameObject playerPrefab, Quaternion rotation, int numberOfPlayers)
    {
        for (int i = 0; i < Mathf.Min(spawnPoints.Count, numberOfPlayers); i++)
        {
            // Instantiation du joueur � partir du prefab sp�cifique � l'�quipe avec la rotation sp�cifi�e
            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, rotation);
        }
    }
}