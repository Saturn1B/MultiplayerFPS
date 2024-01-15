using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class BossManager : MonoBehaviour
{
    public Gate[] gates;
    public int gateOpen;
    public float heal;
    private float startHeal = 10000;

    private bool stepA = false;
    private bool stepB = false;
    private bool stepC = false;

    private void Start()
    {
        heal = startHeal;
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
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1000);
        }
    }
}
