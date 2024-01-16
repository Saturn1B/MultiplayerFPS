using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace Unity.FPS.UI
{
    public class PlayerHealthBar : NetworkBehaviour
    {
        [Tooltip("Image component dispplaying current health")]
        public Image HealthFillImage;

        Health m_PlayerHealth;

        void Start()
        {
            //PlayerCharacterController playerCharacterController =
            //    GetComponentInParent<PlayerCharacterController>();
            //DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, PlayerHealthBar>(
            //    playerCharacterController, this);

            m_PlayerHealth = GetComponentInParent<Health>();
            //DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerHealthBar>(m_PlayerHealth, this,
            //    playerCharacterController.gameObject);
        }

        void Update()
        {
            Debug.Log("HEALTH DISPLAY");
            Debug.Log("health: " + m_PlayerHealth.CurrentHealth);
            // update health bar value
            HealthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
        }
    }
}