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

    public Vector3 startPose;
    public Vector3 noShieldPose;

    public HealthComponent bossHealt;
                                //av laser
    private bool stepA = false; // bumbaaa
    private bool stepB = false; // les 2
    private bool stepC = false;
    private bool setpD = false;

    private GameManager gameManager;

    public TowerBoss[] towers;

    public GameObject shield;

    private bool bossDeath;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        StartCoroutine(WhatForStartBoss());
    }

    public void Update()
    {
        if (!IsServer) return;

        if (bossHealt.currentHealth.Value <= 0 && bossDeath == false)
        {
            bossDeath = true;
            End();
        }

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
        if (towers[gateOpenA].isDestroyed && towers[gateOpenB].isDestroyed && towers[gateOpenC].isDestroyed && towers[gateOpenD].isDestroyed && setpD)
        {
            StartCoroutine(DespawnBossShield());
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
            setpD = true;
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
        //shield.SetActive(false);
        shield.transform.localPosition = noShieldPose;


        yield return new WaitForSeconds(5f);

        if (setpD)
        {
            Debug.Log("Finito plus de shild");
            
        }
        else
        {
            //shield.SetActive(true);
            shield.transform.localPosition = startPose;
            SapwnTower();
        }

    }

    public void End()
    {
        // Le boss est mort
    }


}
