using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JoinCode : NetworkBehaviour
{
    public TMP_Text joinCodeText;

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            StartCoroutine(WaitForJoinCode());
        }
    }

    private IEnumerator WaitForJoinCode()
    {
        while (HostManager.Instance == null || string.IsNullOrEmpty(HostManager.Instance.JoinCode))
        {
            yield return null;
        }

        joinCodeText.text = HostManager.Instance.JoinCode;
    }
}
