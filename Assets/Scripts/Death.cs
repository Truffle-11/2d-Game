using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public string playerTag = "Player";
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag(playerTag)) 
        {
            // Destroy the player object
            Destroy(other.gameObject);
        }
    }
}