using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{       
    public float moveSpeed = 5f;
    public float jumpForce = 4.3f;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);


        float x = Input.GetAxis("Horizontal");


        Vector2 move = transform.right * x;
        Vector2 velocity = move * moveSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;


        if (Input.GetButtonDown("Jump") && isGrounded) {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}


