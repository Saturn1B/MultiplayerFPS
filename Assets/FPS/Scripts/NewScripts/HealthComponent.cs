using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthComponent : NetworkBehaviour
{
    [SerializeField] private bool isInvicible;
    [SerializeField] private float invicibilityTime;
    public float maxHealth;
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public delegate void OnDeathDelegate ();
    public OnDeathDelegate OnDeath;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += (float previousValue, float newValue) =>
        {
            Debug.Log(newValue);

            if (newValue <= 0)
            {
                HandleDeath();
            }
        };
    }

    [ClientRpc]
    public void TakeDamageClientRpc(float amount, string origin)
	{
        if (isInvicible) return;
        Debug.Log("damage isn't ownwer");
        if (!IsOwner) return;
        Debug.Log("damage is ownwer");


        Debug.Log(origin + " inflicted " + amount + " of damages");

        if(amount  <= currentHealth.Value)
		{
            currentHealth.Value -= amount;
		}
	}

    private void HandleDeath()
	{
		if (GetComponent<PlayerController>())
		{
            transform.position = new Vector3(0, 3, 0);
            StartCoroutine(InvincibilityFrame());
            return;
		}
        gameObject.GetComponent<NetworkObject>().Despawn();
        //Destroy(gameObject);
    }

    private IEnumerator InvincibilityFrame()
	{
        isInvicible = true;
        yield return new WaitForSeconds(0.5f);
        currentHealth.Value = maxHealth;
        yield return new WaitForSeconds(invicibilityTime - 0.5f);
        isInvicible = false;
	}

	//private void OnDestroy()
	//{
 //       if(OnDeath != null)
 //           OnDeath.Invoke();
 //   }

	public override void OnNetworkDespawn()
	{
        if (OnDeath != null)
            OnDeath.Invoke();
    }
}
