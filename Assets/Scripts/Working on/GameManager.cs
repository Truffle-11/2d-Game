using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public GameObject lobbyRoot;

    public Transform lobbySpawn;
    public Transform gameSpawn;

    public List<GameObject> maps = new List<GameObject>();

    public float freezeTime = 5f;
    public int winsToEnd = 5;

    private NetworkVariable<int> currentMapIndex = new NetworkVariable<int>(-1);
    private NetworkVariable<bool> roundActive = new NetworkVariable<bool>(false);
    private NetworkVariable<int> roundId = new NetworkVariable<int>(0);

    private NetworkVariable<bool> reviveAllPlayers = new NetworkVariable<bool>(false);

    private bool freezeActive;

    private HashSet<ulong> alivePlayers = new HashSet<ulong>();
    private HashSet<ulong> eliminatedPlayers = new HashSet<ulong>();

    private Dictionary<ulong, int> matchWins = new Dictionary<ulong, int>();
    private Dictionary<ulong, NetworkObject> ghostsByOwner = new Dictionary<ulong, NetworkObject>();

    private Dictionary<ulong, string> baseNameByClient = new Dictionary<ulong, string>();
    private Dictionary<ulong, int> totalWinsByClient = new Dictionary<ulong, int>();

    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        currentMapIndex.OnValueChanged += OnMapChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedServer;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedServer;

            DisableAllMaps();
            currentMapIndex.Value = -1;

            roundActive.Value = false;
            freezeActive = false;
            reviveAllPlayers.Value = false;

            if (lobbyRoot != null) lobbyRoot.SetActive(true);
            SetLobbyActiveClientRpc(true);
        }
        else
        {
            ApplyClientStateFromVars();
        }
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedServer;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedServer;
        }
    }

    void ApplyClientStateFromVars()
    {
        OnMapChanged(-999, currentMapIndex.Value);

        if (lobbyRoot != null)
            lobbyRoot.SetActive(!roundActive.Value);
    }

    public bool IsRoundActive()
    {
        return roundActive.Value;
    }

    public bool IsFreezeActive()
    {
        return freezeActive;
    }

    public int GetRoundId()
    {
        return roundId.Value;
    }

    public bool ReviveAllPlayers()
    {
        return reviveAllPlayers.Value;
    }

    public bool IsClientAlive(ulong clientId)
    {
        return alivePlayers.Contains(clientId);
    }

    public void RegisterBaseNameServer(ulong clientId, string baseName)
    {
        if (!IsServer) return;

        if (string.IsNullOrWhiteSpace(baseName))
            baseName = "Player";

        baseNameByClient[clientId] = baseName;

        if (!totalWinsByClient.ContainsKey(clientId))
            totalWinsByClient[clientId] = 0;
    }

    public string GetBaseNameForClientServer(ulong clientId)
    {
        if (!IsServer) return "Player";

        string v;
        if (baseNameByClient.TryGetValue(clientId, out v))
            return v;

        return "Player";
    }

    public int GetTotalWinsForClientServer(ulong clientId)
    {
        if (!IsServer) return 0;

        int v;
        if (totalWinsByClient.TryGetValue(clientId, out v))
            return v;

        return 0;
    }

    public void ApplyStoredStatsToObjectServer(ulong clientId, GameObject obj)
    {
        if (!IsServer) return;
        if (obj == null) return;

        string bn = GetBaseNameForClientServer(clientId);
        int tw = GetTotalWinsForClientServer(clientId);

        PlayerName pn = obj.GetComponent<PlayerName>();
        if (pn != null)
            pn.SetStatsServer(bn, tw);
    }

    public void RegisterGhostServer(ulong ownerClientId, NetworkObject ghostNetObj)
    {
        if (!IsServer) return;
        if (ghostNetObj == null) return;

        ghostsByOwner[ownerClientId] = ghostNetObj;
    }

    public void HostStartGame()
    {
        if (!IsServer) return;
        if (roundActive.Value) return;

        BeginMatch();
    }

    void BeginMatch()
    {
        alivePlayers.Clear();
        eliminatedPlayers.Clear();
        matchWins.Clear();

        foreach (KeyValuePair<ulong, NetworkClient> kvp in NetworkManager.Singleton.ConnectedClients)
        {
            ulong id = kvp.Key;
            alivePlayers.Add(id);

            if (!baseNameByClient.ContainsKey(id))
                baseNameByClient[id] = "Player";

            if (!totalWinsByClient.ContainsKey(id))
                totalWinsByClient[id] = 0;
        }

        StartNewRound();
    }

    void StartNewRound()
    {
        if (!IsServer) return;

        reviveAllPlayers.Value = false;

        if (alivePlayers.Count <= 0)
        {
            EndToLobby();
            return;
        }

        roundActive.Value = true;

        if (lobbyRoot != null) lobbyRoot.SetActive(false);
        SetLobbyActiveClientRpc(false);

        int newIndex = Random.Range(0, maps.Count);
        currentMapIndex.Value = newIndex;

        roundId.Value++;

        Vector3 spawnPos = gameSpawn != null ? gameSpawn.position : Vector3.zero;

        TeleportAliveServer(spawnPos);
        TeleportAllLocalPlayersClientRpc(spawnPos);

        StartCoroutine(FreezeAliveCoroutine());
    }

    IEnumerator FreezeAliveCoroutine()
    {
        freezeActive = true;
        SetAliveFrozen(true);

        yield return new WaitForSeconds(freezeTime);

        SetAliveFrozen(false);
        freezeActive = false;
    }

    void SetAliveFrozen(bool frozen)
    {
        foreach (ulong clientId in alivePlayers)
        {
            NetworkObject p = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (p == null || !p.IsSpawned) continue;

            PlayerFreeze fr = p.GetComponent<PlayerFreeze>();
            if (fr != null) fr.SetFrozenClientRpc(frozen);
        }
    }

    void TeleportAliveServer(Vector3 pos)
    {
        foreach (ulong clientId in alivePlayers)
        {
            NetworkObject p = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (p == null || !p.IsSpawned) continue;

            p.transform.position = pos;

            Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    void OnMapChanged(int oldValue, int newValue)
    {
        DisableAllMaps();

        if (newValue >= 0 && newValue < maps.Count)
        {
            if (maps[newValue] != null)
                maps[newValue].SetActive(true);
        }
    }

    void DisableAllMaps()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i] != null)
                maps[i].SetActive(false);
        }
    }

    public void PlayerReachedFinish(ulong clientId)
    {
        if (!IsServer) return;
        if (!roundActive.Value) return;
        if (freezeActive) return;

        if (!alivePlayers.Contains(clientId)) return;

        AwardWinAndThen(clientId, false);
    }

    public void PlayerEliminated(ulong clientId)
    {
        if (!IsServer) return;
        if (!roundActive.Value) return;

        // Guard: ignore repeats
        if (!alivePlayers.Contains(clientId))
            return;

        alivePlayers.Remove(clientId);
        eliminatedPlayers.Add(clientId);

        if (alivePlayers.Count == 1)
        {
            bool found = false;
            ulong winnerId = 0;

            foreach (ulong id in alivePlayers)
            {
                winnerId = id;
                found = true;
                break;
            }

            if (found)
                AwardWinAndThen(winnerId, true);
            else
                EndToLobby();

            return;
        }

        if (alivePlayers.Count == 0)
        {
            EndToLobby();
        }
    }

    void AwardWinAndThen(ulong winnerId, bool endToLobby)
    {
        if (!matchWins.ContainsKey(winnerId))
            matchWins[winnerId] = 0;
        matchWins[winnerId]++;

        if (!totalWinsByClient.ContainsKey(winnerId))
            totalWinsByClient[winnerId] = 0;
        totalWinsByClient[winnerId]++;

        NetworkObject winnerPlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(winnerId);
        if (winnerPlayer != null && winnerPlayer.IsSpawned)
        {
            PlayerName pn = winnerPlayer.GetComponent<PlayerName>();
            if (pn == null) pn = winnerPlayer.GetComponentInChildren<PlayerName>(true);
            if (pn != null) pn.AddWinServer();
        }

        if (matchWins[winnerId] >= winsToEnd)
        {
            EndToLobby();
            return;
        }

        if (endToLobby) EndToLobby();
        else StartNewRound();
    }

    void EndToLobby()
    {
        if (!IsServer) return;

        roundActive.Value = false;
        freezeActive = false;

        currentMapIndex.Value = -1;
        DisableAllMaps();

        // Ghosts will revive themselves when this becomes true
        reviveAllPlayers.Value = true;

        if (lobbyRoot != null) lobbyRoot.SetActive(true);
        SetLobbyActiveClientRpc(true);

        Vector3 lobbyPos = lobbySpawn != null ? lobbySpawn.position : Vector3.zero;

        // Teleport local view to lobby; server-authoritative spawns happen via GhostMovement
        TeleportAllLocalPlayersClientRpc(lobbyPos);

        // Reset alive tracking for lobby state
        alivePlayers.Clear();
        eliminatedPlayers.Clear();
        foreach (KeyValuePair<ulong, NetworkClient> kvp2 in NetworkManager.Singleton.ConnectedClients)
        {
            alivePlayers.Add(kvp2.Key);
        }
    }

    void OnClientConnectedServer(ulong clientId)
    {
        if (!IsServer) return;

        if (!baseNameByClient.ContainsKey(clientId))
            baseNameByClient[clientId] = "Player";

        if (!totalWinsByClient.ContainsKey(clientId))
            totalWinsByClient[clientId] = 0;
    }

    void OnClientDisconnectedServer(ulong clientId)
    {
        if (!IsServer) return;

        alivePlayers.Remove(clientId);
        eliminatedPlayers.Remove(clientId);

        if (ghostsByOwner.ContainsKey(clientId))
            ghostsByOwner.Remove(clientId);
    }

    [ClientRpc]
    void SetLobbyActiveClientRpc(bool active, ClientRpcParams rpcParams = default)
    {
        if (lobbyRoot != null)
            lobbyRoot.SetActive(active);
    }

    [ClientRpc]
    void TeleportAllLocalPlayersClientRpc(Vector3 pos)
    {
        if (NetworkManager.Singleton == null) return;
        if (NetworkManager.Singleton.LocalClient == null) return;

        NetworkObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localPlayer == null) return;

        localPlayer.transform.position = pos;

        Rigidbody2D rb = localPlayer.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
    }
}
