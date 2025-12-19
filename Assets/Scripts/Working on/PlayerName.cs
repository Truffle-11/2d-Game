using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerName : NetworkBehaviour
{
    private NetworkVariable<FixedString64Bytes> baseName =
        new NetworkVariable<FixedString64Bytes>(new FixedString64Bytes(""));

    private NetworkVariable<int> winCount =
        new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        // Only submit a name if the server hasn't already set one for this object
        if (IsOwner)
        {
            if (baseName.Value.Length == 0)
            {
                SubmitBaseNameServerRpc(SessionInfo.Username);
            }
        }
    }

    [ServerRpc]
    private void SubmitBaseNameServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        ulong sender = rpcParams.Receive.SenderClientId;

        if (string.IsNullOrWhiteSpace(name))
            name = "Player";

        // Only set if empty (prevents overwriting stats copied from GameManager)
        if (baseName.Value.Length == 0)
            baseName.Value = new FixedString64Bytes(name);

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterBaseNameServer(sender, baseName.Value.ToString());
    }

    public void AddWinServer()
    {
        if (!IsServer) return;
        winCount.Value++;
    }

    public void SetStatsServer(string existingBaseName, int existingWins)
    {
        if (!IsServer) return;

        if (string.IsNullOrWhiteSpace(existingBaseName))
            existingBaseName = "Player";

        baseName.Value = new FixedString64Bytes(existingBaseName);
        winCount.Value = existingWins;
    }

    public string GetDisplayName()
    {
        string n = baseName.Value.ToString();
        if (string.IsNullOrEmpty(n)) n = "Player";

        if (winCount.Value > 0)
            return n + winCount.Value.ToString();

        return n;
    }

    public string GetBaseNameServer()
    {
        return baseName.Value.ToString();
    }

    public int GetWinCountServer()
    {
        return winCount.Value;
    }
}
