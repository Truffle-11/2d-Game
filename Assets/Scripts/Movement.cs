using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public GameObject Move1;
    public GameObject Move2;
    public float walkSpeed = 0.5f;
    public Transform teleportTarget;
    public string TestTag = "TestTag";
    private float walkTimer = 0f;
    private bool usingMove1 = true;
    private bool onIce = false;
    public Rigidbody2D rb;
    public GameObject weapon;
    [HideInInspector] public bool canMove = true;
    private bool facingRight = true;



    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Move1.SetActive(true);
        Move2.SetActive(false);
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (weapon != null)
        {
            Vector3 wScale = weapon.transform.localScale;
            wScale.x *= -1;
            weapon.transform.localScale = wScale;
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);


        float x = Input.GetAxis("Horizontal");

        if (canMove)
        {
            if (!onIce)
            {
                Vector2 move = transform.right * x;
                Vector2 velocity = move * moveSpeed;
                velocity.y = rb.velocity.y;
                rb.velocity = velocity;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }

            if (x > 0 && !facingRight) Flip();
            else if (x < 0 && facingRight) Flip();

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }




        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }



        if (x != 0)
        {
            walkTimer += Time.deltaTime;

            if (walkTimer >= walkSpeed)
            {
                usingMove1 = !usingMove1;
                Move1.SetActive(usingMove1);
                Move2.SetActive(!usingMove1);
                walkTimer = 0f;
            }
        }
        else
        {

            walkTimer = 0f;
            Move1.SetActive(true);
            Move2.SetActive(false);
            usingMove1 = true;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ice"))
        {
            onIce = true;
        }

        if (collision.collider.CompareTag(TestTag))
        {
            transform.position = teleportTarget.position;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ice"))
        {
            onIce = false;
        }
    }

}


