using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Events;

public class ReanimationHandler : NetworkBehaviour
{
    [SerializeField] private GameObject reaUI;
    [SerializeField] private Image fillImage;

	[SerializeField] private float reaSpeed;

    private bool reaActive;

	public UnityEvent reaPlayers;

	public void ActivateReaUi()
	{
        reaUI.SetActive(true);
        reaActive = true;
		fillImage.fillAmount = 0;
	}
	public void DeactivateReaUi()
	{
		reaUI.SetActive(false);
		reaActive = false;
		fillImage.fillAmount = 0;
	}

	private void Update()
	{
		if (reaActive && Input.GetKey(KeyCode.E))
		{
			if(fillImage.fillAmount < 1)
				fillImage.fillAmount += Time.deltaTime * reaSpeed;
			else
			{
				fillImage.fillAmount = 1;
				DeactivateReaUi();
				reaPlayers.Invoke();
				//rea player
			}
		}
		if(reaActive && Input.GetKeyUp(KeyCode.E) && fillImage.fillAmount < 1)
		{
			fillImage.fillAmount = 0;
		}
	}
}
