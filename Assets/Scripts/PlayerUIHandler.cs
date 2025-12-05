using Unity.Netcode;
using UnityEngine;

public class PlayerUIHandler : NetworkBehaviour
{
    public GameObject playerUI;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerUI.SetActive(false);
        }
    }
}