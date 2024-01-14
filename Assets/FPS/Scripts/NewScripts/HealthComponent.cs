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

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

		//if (Input.GetKeyDown(KeyCode.T))
		//{
  //          TakeDamage(1, "here");
		//}
    }

    [ClientRpc]
    public void TakeDamageClientRpc(float amount, string origin)
	{
        if (!IsOwner) return;
        if (isInvicible) return;

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
		}
	}

    private IEnumerator InvincibilityFrame()
	{
        isInvicible = true;
        yield return new WaitForSeconds(0.5f);
        currentHealth.Value = maxHealth;
        yield return new WaitForSeconds(invicibilityTime - 0.5f);
        isInvicible = false;
	}
}
