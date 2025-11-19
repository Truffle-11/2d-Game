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
    private float moveInput;
    private bool jumpRequested;

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

        moveInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner || isDead) return;

        Vector2 velocity = rb.velocity;

        if (Mathf.Abs(moveInput) > 0.001f)
        {
            velocity.x = moveInput * moveSpeed;
        }

        rb.velocity = velocity;

        if (jumpRequested && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        jumpRequested = false;
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
