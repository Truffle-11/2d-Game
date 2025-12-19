using UnityEngine;
using Unity.Netcode;

public class PlayerFreeze : NetworkBehaviour
{
    private bool isFrozen;
    private Rigidbody2D rb;
    private float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            originalGravity = rb.gravityScale;
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    [ClientRpc]
    public void SetFrozenClientRpc(bool frozen)
    {
        isFrozen = frozen;

        if (rb == null) return;

        if (frozen)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        else
        {
            rb.gravityScale = originalGravity;
        }
    }
}
