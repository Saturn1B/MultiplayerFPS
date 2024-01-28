using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeathMatchScore : NetworkBehaviour
{
    [SerializeField] public NetworkVariable<int> playerScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        playerScore.OnValueChanged += ((int old, int current) =>
        {
            //check if score is best
        });
    }

    [ServerRpc(RequireOwnership = false)]
    public void ScoreServerRpc()
    {
        playerScore.Value++;
    }
}
