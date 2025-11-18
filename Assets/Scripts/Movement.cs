using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Movement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 4.3f;
    public float health = 3f;
    public string EnemyTag = "Enemy";
    public GameObject deadPlayer;
    public GameObject PlayerCinaCam;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (PlayerCinaCam != null)
        {
            PlayerCinaCam.SetActive(IsOwner);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (PlayerCinaCam != null)
        {
            PlayerCinaCam.SetActive(IsOwner);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;

        if (other.CompareTag(EnemyTag))
        {
            health--;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (!isDead && health <= 0)
        {
            isDead = true;
            DieServerRpc();
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");

        Vector2 velocity = rb.velocity;

        if (Mathf.Abs(x) > 0.001f)
        {
            velocity.x = x * moveSpeed;
        }

        rb.velocity = velocity;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    [ServerRpc]
    private void DieServerRpc(ServerRpcParams rpcParams = default)
    {
        if (deadPlayer != null)
        {
            GameObject ghost = Instantiate(deadPlayer, transform.position, transform.rotation);
            NetworkObject ghostNetObj = ghost.GetComponent<NetworkObject>();
            if (ghostNetObj != null)
            {
                ghostNetObj.SpawnWithOwnership(OwnerClientId);
            }
        }

        if (PlayerCinaCam != null)
        {
            Destroy(PlayerCinaCam);
        }

        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Despawn(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
