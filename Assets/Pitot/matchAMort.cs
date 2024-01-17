using UnityEngine;
using System.Collections.Generic;

public class matchAMort : MonoBehaviour
{
    public GameObject redPlayerPrefab;
    public GameObject bluePlayerPrefab;

    public List<Transform> redTeamSpawnPoints;
    public List<Transform> blueTeamSpawnPoints;

    public int numberOfPlayersPerTeam = 5;

    private List<GameObject> redPlayers = new List<GameObject>();
    private List<GameObject> bluePlayers = new List<GameObject>();

    public int redScore;
    public int blueScore;

    void Start()
    {
        InstantiatePlayers();
    }

    void InstantiatePlayers()
    {
        InstantiateTeamPlayers(redTeamSpawnPoints, redPlayerPrefab, Quaternion.Euler(0f, 180f, 0f), numberOfPlayersPerTeam, redPlayers);

        InstantiateTeamPlayers(blueTeamSpawnPoints, bluePlayerPrefab, Quaternion.identity, numberOfPlayersPerTeam, bluePlayers);
    }

    void InstantiateTeamPlayers(List<Transform> spawnPoints, GameObject playerPrefab, Quaternion rotation, int numberOfPlayers, List<GameObject> playerList)
    {
        for (int i = 0; i < Mathf.Min(spawnPoints.Count, numberOfPlayers); i++)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, rotation);
            playerList.Add(player);
        }
    }

    void Update()
    {
        // Simulation de l'élimination d'un ennemi aléatoire pour chaque équipe
        if (Input.GetKeyDown(KeyCode.R))
        {
            EliminateRandomPlayer(redPlayers);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            EliminateRandomPlayer(bluePlayers);
        }

        CheckForWin();
    }

    void EliminateRandomPlayer(List<GameObject> playerList)
    {
        if (playerList.Count > 0)
        {
            int randomIndex = Random.Range(0, playerList.Count);
            GameObject playerToRemove = playerList[randomIndex];
            playerList.RemoveAt(randomIndex);
            Destroy(playerToRemove);
        }
    }

    void CheckForWin()
    {
        if (redPlayers.Count == 0)
        {
            blueScore++;
            Debug.Log("Blue team win the round !");
            InstantiatePlayers();

        }
        else if (bluePlayers.Count == 0)
        {
            redScore++;
            Debug.Log("Red team win the round !");
            InstantiatePlayers();
        }

        if(redScore == 3)
        {
            Debug.Log("Red team win the game !");
        }
        else if (blueScore == 3)
        {
            Debug.Log("Blue team win the game !");
        }
    }
}
