using UnityEngine;
using Unity.Netcode;

public class PlayerFreeze : NetworkBehaviour
{
    private bool frozen;

    private Rigidbody2D rb;
    private float originalGravityScale;
    private bool hasOriginalGravity;

    private BoxCollider2D[] colliders;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
            hasOriginalGravity = true;
        }

        // Cache all box colliders on this player
        colliders = GetComponentsInChildren<BoxCollider2D>(true);
    }

    public bool IsFrozen()
    {
        return frozen;
    }

    [ClientRpc]
    public void SetFrozenClientRpc(bool value)
    {
        frozen = value;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            if (!hasOriginalGravity)
            {
                originalGravityScale = rb.gravityScale;
                hasOriginalGravity = true;
            }

            if (frozen)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;
            }
            else
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = originalGravityScale;
            }
        }

        // Enable / disable colliders
        if (colliders == null || colliders.Length == 0)
        {
            colliders = GetComponentsInChildren<BoxCollider2D>(true);
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = !frozen;
            }
        }
    }
}
