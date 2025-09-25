using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float HP = 20f;
    public float Damage = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If hit by player's weapon
        if (collision.CompareTag("Weapon"))
        {
            PlayerAttack playerAttack = collision.GetComponentInParent<PlayerAttack>();
            if (playerAttack != null)
            {
                TakeDamage(playerAttack.Damage);
            }
        }

        // If touching player directly (deal damage to player)
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Damage);
            }
        }
    }

    void TakeDamage(float damage)
    {
        HP -= damage;
        Debug.Log("Enemy took " + damage + " damage. HP left: " + HP);

        if (HP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}