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
    public float maxHealth = 3f;

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

    private PlayerFreeze freeze;

    public GameObject DeadFinger1;
    public GameObject DeadFinger2;
    public GameObject DeadFinger3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        freeze = GetComponent<PlayerFreeze>();

        if (PlayerCinaCam != null)
            PlayerCinaCam.SetActive(IsOwner);
    }

    public override void OnNetworkSpawn()
    {
        freeze = GetComponent<PlayerFreeze>();

        if (PlayerCinaCam != null)
            PlayerCinaCam.SetActive(IsOwner);
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

        UpdateHearts();

        if (!isDead && health <= 0)
        {
            isDead = true;
            DieServerRpc();
            return;
        }

        bool frozenNow = (freeze != null && freeze.IsFrozen());
        if (frozenNow)
        {
            moveInput = 0f;
            jumpRequested = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);

        moveInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpRequested = true;
    }

    private void FixedUpdate()
    {
        if (!IsOwner || isDead) return;

        bool frozenNow = (freeze != null && freeze.IsFrozen());
        if (frozenNow)
        {
            if (rb != null) rb.velocity = Vector2.zero;
            jumpRequested = false;
            return;
        }

        Vector2 velocity = rb.velocity;

        if (Mathf.Abs(moveInput) > 0.001f)
            velocity.x = moveInput * moveSpeed;
        else
            velocity.x = 0f;

        rb.velocity = velocity;

        if (jumpRequested && isGrounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpRequested = false;
    }

    [ServerRpc]
    private void DieServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong deadClientId = rpcParams.Receive.SenderClientId;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerEliminated(deadClientId);
        }

        string baseN = "Player";
        int wins = 0;

        if (GameManager.Instance != null)
        {
            baseN = GameManager.Instance.GetBaseNameForClientServer(deadClientId);
            wins = GameManager.Instance.GetTotalWinsForClientServer(deadClientId);
        }

        NetworkObject ghostNetObj = null;

        if (deadPlayer != null)
        {
            GameObject ghost = Instantiate(deadPlayer, transform.position, transform.rotation);

            PlayerName ghostName = ghost.GetComponent<PlayerName>();
            if (ghostName != null)
            {
                ghostName.SetStatsServer(baseN, wins);
            }

            ghostNetObj = ghost.GetComponent<NetworkObject>();
            if (ghostNetObj != null)
            {
                ghostNetObj.SpawnWithOwnership(deadClientId);
            }
        }

        if (GameManager.Instance != null && ghostNetObj != null)
        {
            GameManager.Instance.RegisterGhostServer(deadClientId, ghostNetObj);
        }

        if (PlayerCinaCam != null)
        {
            Destroy(PlayerCinaCam);
        }

        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj != null)
            netObj.Despawn(true);
        else
            Destroy(gameObject);
    }

    // Called by GhostMovement when reviving into lobby
    public void ResetHealthServer()
    {
        if (!IsServer) return;

        health = maxHealth;
        isDead = false;
    }

    void UpdateHearts()
    {
        if (DeadFinger1 == null || DeadFinger2 == null || DeadFinger3 == null) return;

        if (health >= 3)
        {
            DeadFinger1.SetActive(false);
            DeadFinger2.SetActive(false);
            DeadFinger3.SetActive(false);
        }
        else if (health == 2)
        {
            DeadFinger1.SetActive(true);
            DeadFinger2.SetActive(false);
            DeadFinger3.SetActive(false);
        }
        else if (health == 1)
        {
            DeadFinger1.SetActive(true);
            DeadFinger2.SetActive(true);
            DeadFinger3.SetActive(false);
        }
        else
        {
            DeadFinger1.SetActive(true);
            DeadFinger2.SetActive(true);
            DeadFinger3.SetActive(true);
        }
    }
}
