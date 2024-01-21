using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class BossManager : NetworkBehaviour
{
    public Gate[] gates;
    private int gateOpenA = 0;
    private int gateOpenB = 1;
    private int gateOpenC = 2;
    private int gateOpenD = 3;
    private float startHeal;
                                //av laser
    private bool stepA = false; // bumbaaa
    private bool stepB = false; // les 2 
    private bool stepC = false; 

    private GameManager gameManager;

    public TowerBoss[] towers;

    public GameObject shield;
 
    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        StartCoroutine(WhatForStartBoss());
    }

    public void Update()
    {
        if (!IsServer) return;

        if (towers[gateOpenA].isDestroyed && stepA == false)
        {
            stepA = true;  
            StartCoroutine(DespawnBossShield());
            gates[gateOpenA].GoClose();
        }
        if (towers[gateOpenA].isDestroyed && towers[gateOpenB].isDestroyed && stepB == false)
        {
            stepB = true; 
            StartCoroutine(DespawnBossShield());
            gates[gateOpenA].GoClose(); gates[gateOpenB].GoClose();
        }
        if (towers[gateOpenA].isDestroyed && towers[gateOpenB].isDestroyed && towers[gateOpenC].isDestroyed && stepC == false)
        {
            stepC = true;
            StartCoroutine(DespawnBossShield());
            gates[gateOpenA].GoClose(); gates[gateOpenB].GoClose(); gates[gateOpenC].GoClose();
        }

    }

    private void RedyToStart()
    {
        if (!IsServer) return;

        gates[gateOpenA].GoOpen();
        towers[gateOpenA].modAttack = true;
    }

    private void SapwnTower()
    {
        if (!IsServer) return;

        if (stepA && !stepB && !stepC)
        {
            for (int i = 0; i < 2; i++)
            {
                gates[i].GoOpen();
                towers[i].GetComponent<HealthComponent>().currentHealth.Value = 100;
                towers[i].modAttack = true;
                towers[i].fx.SetActive(true);
                towers[i].isDestroyed = false;
            }
            return;
        }
        if (stepA && stepB && !stepC)
        {
            for (int i = 0; i < 3; i++)
            {
                gates[i].GoOpen();
                towers[i].GetComponent<HealthComponent>().currentHealth.Value = 100;
                towers[i].modAttack = true;
                towers[i].fx.SetActive(true);
                towers[i].isDestroyed = false;
            }
            return;
        }
        if (stepA && stepB && stepC)
        {
            for (int i = 0; i < towers.Length; i++)
            {
                gates[i].GoOpen();
                towers[i].GetComponent<HealthComponent>().currentHealth.Value = 100;
                towers[i].modAttack = true;
                towers[i].fx.SetActive(true);
                towers[i].isDestroyed = false;
            }
            return;
        }
        
        
    }

    private IEnumerator WhatForStartBoss()// attend que les joueur charge 
    {
        while (!gameManager.canPlayerMove.Value)
        {
            yield return null;

        }

        RedyToStart(); Debug.Log("get redy for start");
    }

    private IEnumerator DespawnBossShield()// fait despawn de shield du boss apres qu'un tower soit destroy
    {
        shield.SetActive(false);

        yield return new WaitForSeconds(30f);

        shield.SetActive(true);

        SapwnTower();
    }

    
}
