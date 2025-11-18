using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject PauseMenu;
    private bool IsPaused = false;

    public void Leave()
    {
        StartCoroutine(LeaveGame());
    }

    IEnumerator LeaveGame()
    {
        Debug.Log("You Left");
        NetworkManager.Singleton.Shutdown();
        yield return null;
        SceneManager.LoadScene(0);      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsPaused == false)
        {
            PauseMenu.SetActive(true);
            IsPaused = true;
            Debug.Log("IsPuased = true");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsPaused == true)
        {
            PauseMenu.SetActive(false);
            IsPaused = false;
            Debug.Log("IsPuased = false");
        }
    }
}
