using Unity.Netcode;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class MainMenuDisplay : MonoBehaviour
{
    public GameObject connectingPanel;
    public GameObject menuPanel;
    public TMP_InputField joinCodeInputField;

    private async void Start()
    {
        try
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
            }
            else
            {
                Debug.Log("Player already signed in.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }


    public void StartHost()
    {
        HostManager.Instance.StartHost();
    }

    public void StartClient()
    {
        ClientManager.Instance.StartClient(joinCodeInputField.text);
    }

    public void LeaveGame()
    {
        Debug.Log("Left Game");
        Application.Quit();
    }
}
