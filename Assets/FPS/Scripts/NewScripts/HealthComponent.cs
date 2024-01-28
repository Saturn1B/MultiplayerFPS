using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class HealthComponent : NetworkBehaviour
{
    [SerializeField] private bool isInvicible;
    [SerializeField] private float invicibilityTime;
    public float maxHealth;
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public delegate void OnDeathDelegate ();
    public OnDeathDelegate OnDeath;
    [SerializeField] private bool dontDestroy;

    private GameManager gameManager;

    private int currentEnemyId;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += (float previousValue, float newValue) =>
        {
            Debug.Log(newValue);

            if (newValue <= 0 && newValue != previousValue)
            {
                HandleDeath();
            }
        };
    }

    [ClientRpc]
    public void TakeDamageClientRpc(float amount)
	{
        if (isInvicible) return;
        Debug.Log("damage isn't ownwer");
        if (!IsOwner) return;
        Debug.Log("damage is ownwer");

        Debug.Log("Inflicted " + amount + " of damages");

        if (amount <= currentHealth.Value)
        {
            currentHealth.Value -= amount;
        }
        else
            currentHealth.Value = 0;
    }
    [ClientRpc]
    public void TakeDamageClientRpc(float amount, int enemyId)
    {
        if (isInvicible) return;
        Debug.Log("damage isn't ownwer");
        if (!IsOwner) return;
        Debug.Log("damage is ownwer");

        currentEnemyId = enemyId;

        Debug.Log("Inflicted " + amount + " of damages");

        if (amount <= currentHealth.Value)
        {
            currentHealth.Value -= amount;
        }
        else
            currentHealth.Value = 0;
    }

    private void HandleDeath()
	{
		if (GetComponent<PlayerNetworkHandler>())
		{
            if (gameManager == null)
                gameManager = FindObjectOfType<GameManager>();

            if(gameManager._currentGameMode.Value == 0)
			{
                GetComponent<PlayerNetworkHandler>().PlayerDown();
			}
            else if (gameManager._currentGameMode.Value == 2)
            {
                PlayerNetworkHandler[] players = FindObjectsOfType<PlayerNetworkHandler>();

                foreach (PlayerNetworkHandler player in players)
                {
                    if (int.Parse(player.OwnerClientId.ToString()) == currentEnemyId)
                        player.transform.GetComponent<DeathMatchScore>().ScoreServerRpc();
                }

                GetComponent<PlayerNetworkHandler>().ChooseSpawn();
                StartCoroutine(DeathmatchRespawn());
            }
            else if(gameManager._currentGameMode.Value == 3)
            {
                if(gameObject.CompareTag("TeamA"))
                    gameManager.TeamScoreServerRpc(1);
                else if (gameObject.CompareTag("TeamB"))
                    gameManager.TeamScoreServerRpc(0);

                GetComponent<PlayerNetworkHandler>().ChooseSpawn();
                StartCoroutine(DeathmatchRespawn());
            }
            else
			{
                GetComponent<PlayerNetworkHandler>().ChooseSpawn();
                StartCoroutine(InvincibilityFrame());
            }
            return;
		}

        if (dontDestroy)
        {

        }
        else
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
            //Destroy(gameObject);
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

    private IEnumerator DeathmatchRespawn()
    {
        isInvicible = true;
        GetComponent<PlayerNetworkHandler>().PlayerDown();
        //hide player and tp him to the player who killed him
        yield return new WaitForSeconds(5f);
        currentHealth.Value = maxHealth;
        isInvicible = false;
        GetComponent<PlayerNetworkHandler>().PlayerUpServerRpc();
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
