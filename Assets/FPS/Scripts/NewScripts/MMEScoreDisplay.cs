using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class MMEScoreDisplay : NetworkBehaviour
{
    public TMP_Text scoreTextA;
    public TMP_Text scoreTextB;
    public GameObject scoreHolder;

    private GameManager gameManager;

    public void Setup()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.scoreTeamA.OnValueChanged += (int old, int current) =>
        {
            UpdateScoreA(current);
        };
        gameManager.scoreTeamB.OnValueChanged += (int old, int current) =>
        {
            UpdateScoreB(current);
        };
    }

    private void UpdateScoreA(int score)
    {
        scoreTextA.text = score.ToString();
    }

    private void UpdateScoreB(int score)
    {
        scoreTextB.text = score.ToString();
    }
}
