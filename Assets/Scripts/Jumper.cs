using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public float launchForce = 15f; // Adjust to tune jump strength
    public string playerTag = "Player"; // Tag your player accordingly

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                
                rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
            }
        }
    }
}