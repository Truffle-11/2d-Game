using UnityEngine;
using Unity.Netcode;

public class GhostMovement : NetworkBehaviour
{
    public float moveSpeed = 8f;
    public float flySpeed = 5f;

    public Rigidbody2D rb;
    public GameObject ghostCinaCam;

    public GameObject playerPrefab;

    private bool reviveStarted;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (ghostCinaCam != null)
            ghostCinaCam.SetActive(IsOwner);
    }

    public override void OnNetworkSpawn()
    {
        if (ghostCinaCam != null)
            ghostCinaCam.SetActive(IsOwner);
    }

    void Update()
    {
        if (IsServer)
        {
            TryReviveServer();
        }

        if (!IsOwner) return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 v = rb.velocity;
        v.x = x * moveSpeed;
        v.y = y * flySpeed;
        rb.velocity = v;
    }

    void TryReviveServer()
    {
        if (reviveStarted) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.ReviveAllPlayers()) return;

        reviveStarted = true;

        if (playerPrefab == null)
        {
            Debug.LogError("GhostMovement: playerPrefab not assigned on ghost prefab.");
            return;
        }

        ulong clientId = OwnerClientId;

        NetworkObject existingPlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (existingPlayer != null && existingPlayer.IsSpawned)
            return;

        Vector3 spawnPos = GameManager.Instance.lobbySpawn != null
            ? GameManager.Instance.lobbySpawn.position
            : Vector3.zero;

        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        NetworkObject playerNet = playerObj.GetComponent<NetworkObject>();
        playerNet.SpawnAsPlayerObject(clientId, true);

        Movement mv = playerObj.GetComponent<Movement>();
        if (mv != null)
        {
            mv.ResetHealthServer();
        }

        GameManager.Instance.ApplyStoredStatsToObjectServer(clientId, playerObj);

        NetworkObject myNet = GetComponent<NetworkObject>();
        if (myNet != null && myNet.IsSpawned)
            myNet.Despawn(true);
        else
            Destroy(gameObject);
    }
}
