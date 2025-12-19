using UnityEngine;
using Unity.Netcode;

public class RoundTrigger : MonoBehaviour
{
    private bool triggered;
    private int lastRoundId = -1;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsServer) return;

        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.IsRoundActive()) return;
        if (GameManager.Instance.IsFreezeActive()) return;

        int r = GameManager.Instance.GetRoundId();
        if (r != lastRoundId)
        {
            lastRoundId = r;
            triggered = false;
        }

        if (triggered) return;

        NetworkObject netObj = collision.GetComponent<NetworkObject>();
        if (netObj == null) return;

        Movement movement = collision.GetComponent<Movement>();
        if (movement == null) return;

        ulong ownerId = netObj.OwnerClientId;
        if (!GameManager.Instance.IsClientAlive(ownerId)) return;

        triggered = true;
        GameManager.Instance.PlayerReachedFinish(ownerId);
    }
}
