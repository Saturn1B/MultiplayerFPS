using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class BossManager : MonoBehaviour
{
    public Gate[] gates;
    public int gateOpen;
    public float heal;
    private float startHeal = 10000;

                                //av laser
    private bool stepA = false; // bumbaaa
    private bool stepB = false; // les 2 
    private bool stepC = false; // 

    

    public bool modAttack = false;
    //public PlayerNetworkHandler[] playerNetworkHandler;
    public GameObject[] players;
    private int maTarget;
    public Transform shooter;
    public Transform weapon;

    public GameObject towerA;
    public GameObject towerB;

    public float rotationSpeed = 2.0f;


    private void Start()
    {
        heal = startHeal;

        maTarget = Random.Range(0, players.Length);

        StartCoroutine(ChangeTargets());
    }


    public void TakeDamage(float damage)
    {
        heal -= damage;
        Debug.Log(heal);

        if (heal <= startHeal * 0.1 && stepC == false)
        {
            Debug.Log("Ta perdu 90%");stepC = true;
            gates[gateOpen].GoClose();
            return;
        }
        if (heal <= startHeal * 0.4 && stepB == false)
        {
            Debug.Log("Ta perdu 60%"); stepB= true;
            gates[gateOpen].GoClose();
            return;
        }
        if (heal <= startHeal * 0.7 && stepA == false)
        {
            Debug.Log("Ta perdu 30%"); stepA= true;
            gates[gateOpen].GoClose();
            return;
        }

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            int rand = Random.Range(0, gates.Length);

            this.gameObject.transform.SetParent(gates[rand].bossPosition.transform);
            this.gameObject.transform.localPosition = Vector3.zero;
            this.gameObject.transform.localPosition = new Vector3(0,-2.7f,0);

            gates[rand].GoOpen();
            gateOpen = rand;

            modAttack = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1000);
        }

        TargetPlayer(maTarget);
    }
    
    private void TargetPlayer(int num)
    {
        // Calculer la direction vers le joueur
        Vector3 directionToPlayer = players[num].transform.position - weapon.position;

        // Calculer la rotation pour faire face à la direction du joueur en douceur
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Interpoler en douceur entre la rotation actuelle et la rotation cible
        weapon.rotation = Quaternion.Slerp(weapon.rotation, targetRotation, Time.deltaTime * rotationSpeed);



        // Obtenez la direction du canon
        Vector3 canonDirection = weapon.forward;

        // Déclarez une variable RaycastHit pour stocker les informations sur l'objet touché par le rayon
        RaycastHit hit;

        // Utilisez Physics.Raycast pour tirer le rayon depuis la position du canon dans la direction du canon
        if (Physics.Raycast(weapon.position, canonDirection, out hit))
        {
            if (/*hit.transform.GetComponent<PlayerNetworkHandler>()*/ hit.transform.CompareTag("Player"))
            {
                // Le joueur est détecté, arrête le mouvement et vise le joueur

                Debug.Log("beeeeeteuuu");
            }


        }

        // Dessine une ligne représentant le raycast dans l'éditeur pour le débogage
        Debug.DrawRay(weapon.position, canonDirection * 100, Color.green);
    }


    public IEnumerator ChangeTargets() 
    { 
        yield return new WaitForSeconds(Random.Range(8, 20));

        maTarget = Random.Range(0, players.Length);

        Debug.Log("Boss change de target" + maTarget);

        StartCoroutine(ChangeTargets());
    }
}
