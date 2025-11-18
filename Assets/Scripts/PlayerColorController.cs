using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerColorController : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text usernameText;

    private NetworkVariable<Color> netColor = new NetworkVariable<Color>(
        Color.white,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private NetworkVariable<FixedString32Bytes> netUsername =
        new NetworkVariable<FixedString32Bytes>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        // Subscribe
        netColor.OnValueChanged += OnColorChanged;
        netUsername.OnValueChanged += OnUsernameChanged;

        // Apply current values (for late joiners)
        OnColorChanged(Color.white, netColor.Value);
        OnUsernameChanged(default, netUsername.Value);

        // Only the owner tells the server their appearance
        if (IsOwner && IsClient)
        {
            Debug.Log("Owner " + OwnerClientId + " sending appearance. Color: " +
                      SessionInfo.PlayerColor + " Username: " + SessionInfo.Username);

            SubmitAppearanceServerRpc(SessionInfo.PlayerColor, SessionInfo.Username);
        }
    }

    [ServerRpc]
    private void SubmitAppearanceServerRpc(Color color, string username)
    {
        Debug.Log("Server received appearance from client. Color: " + color +
                  " Username: " + username);

        netColor.Value = color;
        netUsername.Value = username;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
        }
        else
        {
            Debug.LogError("SpriteRenderer not assigned on " + gameObject.name);
        }
    }

    private void OnUsernameChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        if (usernameText != null)
        {
            usernameText.text = newValue.ToString();
        }
        else
        {
            Debug.LogWarning("UsernameText not assigned on " + gameObject.name);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        netColor.OnValueChanged -= OnColorChanged;
        netUsername.OnValueChanged -= OnUsernameChanged;
    }
}
