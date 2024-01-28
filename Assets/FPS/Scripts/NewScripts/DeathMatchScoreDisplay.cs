using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class DeathMatchScoreDisplay : NetworkBehaviour
{
    public TMP_Text scoreText;
    public GameObject scoreHolder;

    private DeathMatchScore matchScore;


    public override void OnNetworkSpawn()
    {
        if (matchScore == null) matchScore = GetComponentInParent<DeathMatchScore>();
        matchScore.playerScore.OnValueChanged += (int old, int current) =>
        {
            UpdateScore(current);
        };
        UpdateScore(0);
    }

    void Start()
    {
        matchScore = GetComponentInParent<DeathMatchScore>();
    }

    private void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
