using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerHealthDisplay : NetworkBehaviour
{
    [Tooltip("Image component dispplaying current health")]
    public Image HealthFillImage;

    private HealthComponent health;

    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(IsLocalPlayer);
        if (health == null) health = GetComponentInParent<HealthComponent>();
        health.currentHealth.OnValueChanged += (float previousValue, float newValue) =>
        {
            UpdateHealthBar(newValue, health.maxHealth);
        };
    }

    void Start()
    {
        health = GetComponentInParent<HealthComponent>();
    }

    void UpdateHealthBar(float current, float max)
    {
        // update health bar value
        HealthFillImage.fillAmount = current / max;
    }
}
