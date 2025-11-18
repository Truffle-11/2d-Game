using System.Collections;
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

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        yield return null;

        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsPaused)
        {
            PauseMenu.SetActive(true);
            IsPaused = true;
            Debug.Log("IsPaused = true");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsPaused)
        {
            PauseMenu.SetActive(false);
            IsPaused = false;
            Debug.Log("IsPaused = false");
        }
    }
}
