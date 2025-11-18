using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GhostMovement : NetworkBehaviour
{
    public float moveSpeed = 8f;
    public float flySpeed = 5f;
    public Rigidbody2D rb;
    public GameObject ghostCinaCam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (ghostCinaCam != null)
        {
            ghostCinaCam.SetActive(IsOwner);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (ghostCinaCam != null)
        {
            ghostCinaCam.SetActive(IsOwner);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 velocity = rb.velocity;

        if (Mathf.Abs(x) > 0.001f)
        {
            velocity.x = x * moveSpeed;
        }

        if (Mathf.Abs(y) > 0.001f)
        {
            velocity.y = y * flySpeed;
        }

        rb.velocity = velocity;
    }
}
