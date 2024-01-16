using UnityEngine;
using System.Collections.Generic;

public class matchAMort : MonoBehaviour
{
    public GameObject redPlayerPrefab; // Préfab du joueur de l'équipe rouge
    public GameObject bluePlayerPrefab; // Préfab du joueur de l'équipe bleue

    public List<Transform> redTeamSpawnPoints;
    public List<Transform> blueTeamSpawnPoints;

    public int numberOfPlayersPerTeam = 5; // Nombre de joueurs par équipe

    void Start()
    {
        InstantiatePlayers();
    }

    void InstantiatePlayers()
    {
        // Instantiate les joueurs pour l'équipe rouge avec une rotation de 180 degrés
        InstantiateTeamPlayers(redTeamSpawnPoints, redPlayerPrefab, Quaternion.Euler(0f, 180f, 0f), numberOfPlayersPerTeam);

        // Instantiate les joueurs pour l'équipe bleue
        InstantiateTeamPlayers(blueTeamSpawnPoints, bluePlayerPrefab, Quaternion.identity, numberOfPlayersPerTeam);
    }

    void InstantiateTeamPlayers(List<Transform> spawnPoints, GameObject playerPrefab, Quaternion rotation, int numberOfPlayers)
    {
        for (int i = 0; i < Mathf.Min(spawnPoints.Count, numberOfPlayers); i++)
        {
            // Instantiation du joueur à partir du prefab spécifique à l'équipe avec la rotation spécifiée
            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, rotation);
        }
    }
}