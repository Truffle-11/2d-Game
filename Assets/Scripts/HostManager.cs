using System;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;

public class HostManager : MonoBehaviour
{
    public int maxConnections = 6;
    public string gameplaySceneName = "2D GAME OF HELL";
    public string JoinCode { get; private set; }
    public static HostManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async void StartHost()
    {
        Allocation allocation;

        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError("Relay create allocation request failed " + e.Message);
            return;
        }

        try
        {
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.LogError("Relay get join code request failed " + e.Message);
            return;
        }

        Debug.Log("Host got join code: " + JoinCode);
        SessionInfo.JoinCode = JoinCode;

        var relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.LogError("Failed to start host.");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(
            gameplaySceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }
}
