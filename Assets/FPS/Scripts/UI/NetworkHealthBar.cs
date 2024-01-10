using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace Unity.FPS.UI
{
    public class NetworkHealthBar : NetworkBehaviour
    {
        [Tooltip("Image component dispplaying current health")]
        public Image HealthFillImage;
        public GameObject HealthBar;

        public Health m_PlayerHealth;

        public override void OnNetworkSpawn()
        {
            HealthBar.SetActive(!IsLocalPlayer);
        }

        void Start()
        {
            m_PlayerHealth = GetComponent<Health>();
        }

        void Update()
        {
            // update health bar value
            HealthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
        }
    }
}
