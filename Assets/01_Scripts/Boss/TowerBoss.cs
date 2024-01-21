using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerBoss : NetworkBehaviour
{
    private float startHeal;

    public bool modAttack = false;
    private bool attack = false;

    public PlayerNetworkHandler[] players;
    private int maTarget;

    public GameObject towerA;
    public Transform shooterA;
    public Transform weaponA;

    public float rotationSpeed = 2.0f;

    private GameManager gameManager;
    private HealthComponent healthComponent;

    public bool isDestroyed = false;
    
    private void Start()
    {
        healthComponent = gameObject.GetComponent<HealthComponent>();

        StartCoroutine(ChangeTargets());

        gameManager = FindAnyObjectByType<GameManager>();

        StartCoroutine(TryGetPlayer());

        startHeal = healthComponent.currentHealth.Value;
    }

    public void Update()
    {
        if (!IsServer) return;
      
        if (gameManager.canPlayerMove.Value && modAttack)
        {
            TargetPlayer(maTarget);
        }

        if (healthComponent.currentHealth.Value <= 0 && isDestroyed == false)
        {
            isDestroyed = true;
            modAttack = false;
        }
    }

    private void TargetPlayer(int num)
    {
        // Calculer la direction vers le joueur
        Vector3 directionToPlayer = players[num].transform.position + Vector3.up - weaponA.position;

        // Calculer la rotation pour faire face à la direction du joueur en douceur
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Interpoler en douceur entre la rotation actuelle et la rotation cible
        weaponA.rotation = Quaternion.Slerp(weaponA.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Obtenez la direction du canon
        Vector3 canonDirection = weaponA.forward;

        // Déclarez une variable RaycastHit pour stocker les informations sur l'objet touché par le rayon
        RaycastHit hit;

        // Utilisez Physics.Raycast pour tirer le rayon depuis la position du canon dans la direction du canon
        if (Physics.Raycast(weaponA.position, canonDirection, out hit))
        {
            if (hit.transform.GetComponent<PlayerNetworkHandler>())
            {
                // Le joueur est détecté, arrête le mouvement et vise le joueur
                if (attack == false)
                {
                    StartCoroutine(ChargeDamage(hit.transform.GetComponent<PlayerNetworkHandler>()));
                }
                Debug.Log("beeeeeteuuu");
            }


        }

        // Dessine une ligne représentant le raycast dans l'éditeur pour le débogage
        Debug.DrawRay(weaponA.position, canonDirection * 100, Color.green);
    }


    public IEnumerator ChangeTargets()
    {
        yield return new WaitForSeconds(Random.Range(8, 20));

        maTarget = Random.Range(0, players.Length);

        Debug.Log("Boss change de target" + maTarget);

        StartCoroutine(ChangeTargets());
    }


    public IEnumerator ChargeDamage(PlayerNetworkHandler pl)
    {
        attack = true;

        yield return new WaitForSeconds(Random.Range(1, 4));

        pl.GetComponent<HealthComponent>().TakeDamageClientRpc(2f);

        Debug.Log("Boss charge damage " + 2);

        attack = false;
    }

    private IEnumerator TryGetPlayer()
    {

        while (!gameManager.canPlayerMove.Value)
        {
            yield return null;

        }

        players = FindObjectsOfType<PlayerNetworkHandler>();

        maTarget = Random.Range(0, players.Length);
    }


}
