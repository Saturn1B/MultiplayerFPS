using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;

public class CustomWeapon : MonoBehaviour
{
    [SerializeField] protected Transform startPoint;

    //TO DO magazine management
    //TO DO reload
    //TO DO sound
    //TO DO link to netcode

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    //needs to be a ServerRPC
    protected virtual void Shoot()
	{

    }

    protected virtual void Hit(RaycastHit hit)
	{
        if (hit.transform.GetComponent<Damageable>())
        {
            hit.transform.GetComponent<Damageable>().InflictDamage(60, false, this.gameObject);

        }
    }
}
