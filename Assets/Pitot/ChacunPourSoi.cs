using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChacunPourSoi : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject healBonusPrefab;
    public Transform[] spawnPoints;

    public float healBonusChance = 0.5f;
    public int maxPlayers = 10;
    public int killsToWin = 5;

    private List<Transform> availableSpawnPoints = new List<Transform>();
    private List<GameObject> players = new List<GameObject>();
    private Dictionary<GameObject, int> playerKills = new Dictionary<GameObject, int>();

    void Start()
    {
        InitializeSpawnPoints();
        InstantiatePlayers();
    }

    void Update()
    {
        // Éliminer un joueur aléatoire en appuyant sur "B"
        if (Input.GetKeyDown(KeyCode.B))
        {
            EliminateRandomPlayer();
        }
        CheckWinCondition();
    }

    void InitializeSpawnPoints()
    {
        availableSpawnPoints.AddRange(spawnPoints);
    }

    void InstantiatePlayers()
    {
        int numPlayers = Mathf.Min(maxPlayers, availableSpawnPoints.Count);

        for (int i = 0; i < numPlayers; i++)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomIndex];

            GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            players.Add(player);
            playerKills.Add(player, 0); // Initialiser le nombre de kills à 0

            availableSpawnPoints.RemoveAt(randomIndex);
        }
    }

    void EliminateRandomPlayer()
    {
        if (players.Count > 0)
        {
            int randomIndex = Random.Range(0, players.Count);
            GameObject playerToEliminate = players[randomIndex];
            players.RemoveAt(randomIndex);

            // élimination du joueur (désactivez plutôt que de détruire)
            playerToEliminate.SetActive(false);

            // Ajoutez la logique pour le bonus de heal avec une chance définie
            if (Random.value <= healBonusChance)
            {
                Instantiate(healBonusPrefab, playerToEliminate.transform.position, Quaternion.identity);
            }

            // Augmentez le nombre de kills du joueur éliminé
            playerKills[playerToEliminate]++;

            // Lance la coroutine pour réapparition après 5 secondes
            StartCoroutine(RespawnPlayerAfterDelay(playerToEliminate));
        }
    }

    IEnumerator RespawnPlayerAfterDelay(GameObject playerToRespawn)
    {
        yield return new WaitForSeconds(5f);

            if (availableSpawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform respawnPoint = availableSpawnPoints[randomIndex];

                playerToRespawn.transform.position = respawnPoint.position;

                availableSpawnPoints.RemoveAt(randomIndex);
                players.Add(playerToRespawn);

                playerToRespawn.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No available spawn points for respawn.");
            }
    }

    void CheckWinCondition()
    {
        foreach (var player in playerKills.Keys)
        {
            if (playerKills[player] >= killsToWin)
            {
                Debug.Log(player.name + " wins!");
            }
        }
    }
}
