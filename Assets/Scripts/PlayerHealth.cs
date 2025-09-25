using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float HP = 50f;
    public Transform respawnPoint;
    public float respawnDelay = 5f;
    public Movement playerMovement;

    public Rigidbody2D rb;
    public Collider2D mainCollider;
    public GameObject[] disableOnDeath;
    private float originalGravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    public void TakeDamage(float damage)
    {
        if (HP <= 0) return;

        HP -= damage;
        if (HP <= 0)
        {
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        if (mainCollider != null) mainCollider.enabled = false;

        foreach (GameObject obj in disableOnDeath)
        {
            if (obj != null) obj.SetActive(false);
        }

        if (playerMovement != null) playerMovement.canMove = false;

        yield return new WaitForSeconds(respawnDelay);

        HP = 50f;
        transform.position = respawnPoint.position;

        if (mainCollider != null) mainCollider.enabled = true;

        foreach (GameObject obj in disableOnDeath)
        {
            if (obj != null) obj.SetActive(true);
        }

        if (playerMovement != null) playerMovement.canMove = true;

        rb.gravityScale = originalGravity;
    }

}
