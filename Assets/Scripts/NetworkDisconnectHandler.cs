using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkDisconnectHandler : MonoBehaviour
{
    [SerializeField] private int mainMenuSceneIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (NetworkManager.Singleton == null)
            return;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Disconnected from host. Returning to main menu.");
                SceneManager.LoadScene(mainMenuSceneIndex);
            }
        }
    }
}
