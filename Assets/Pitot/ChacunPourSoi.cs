using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChacunPourSoi : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private List<Transform> availableSpawnPoints = new List<Transform>();
    private List<GameObject> players = new List<GameObject>();

    void Start()
    {
        InitializeSpawnPoints();
        InstantiatePlayers();
    }

    void Update()
    {
        // �liminer un joueur al�atoire en appuyant sur "B"
        if (Input.GetKeyDown(KeyCode.B))
        {
            EliminateRandomPlayer();
        }
    }

    void InitializeSpawnPoints()
    {
        availableSpawnPoints.AddRange(spawnPoints);
    }

    void InstantiatePlayers()
    {
        for (int i = 0; i < 10; i++)
        {
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Not enough spawn points for all players!");
                break;
            }

            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomIndex];

            GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            players.Add(player);

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

            // Ajoutez ici la logique pour l'�limination du joueur (d�sactivez plut�t que de d�truire)
            playerToEliminate.SetActive(false);

            // Lance la coroutine pour r�apparition apr�s 5 secondes
            StartCoroutine(RespawnPlayerAfterDelay(playerToEliminate));
        }
    }

    IEnumerator RespawnPlayerAfterDelay(GameObject playerToRespawn)
    {
        yield return new WaitForSeconds(5f);

        // V�rifie si le GameObject existe toujours
        if (playerToRespawn != null)
        {
            // Trouve un point de spawn disponible
            if (availableSpawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform respawnPoint = availableSpawnPoints[randomIndex];

                // R�apparition du joueur sur le point de spawn
                playerToRespawn.transform.position = respawnPoint.position;

                // Ajoutez ici d'autres actions si n�cessaire

                availableSpawnPoints.RemoveAt(randomIndex);
                players.Add(playerToRespawn);

                // R�active le joueur
                playerToRespawn.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No available spawn points for respawn.");
            }
        }
        else
        {
            Debug.LogWarning("Player to respawn is null.");
        }
    }
}
