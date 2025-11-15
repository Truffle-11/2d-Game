using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 4.3f;
    public float health = 3f;
    public string EnemyTag = "Enemy";
    public GameObject deadPlayer;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(EnemyTag))
        {
            health--;
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            deadPlayer.SetActive(true);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);


        float x = Input.GetAxis("Horizontal");

        Vector2 velocity = rb.velocity;

        if (Mathf.Abs(x) > 0.001f)
        {
            velocity.x = x * moveSpeed;
        }

        rb.velocity = velocity;


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}