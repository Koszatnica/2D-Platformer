using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Komponety")]
    public Rigidbody2D rb;  
    public PlayerInput playerInput;
    public Animator anim;

    [Header("Zmienne Ruchu")]
    public float speed; 
    public float jumpf; 
    public int faceing = 1; 
    public float jumpCutMultiplier = .5f;

    [Header("Zmienne Grawitacji")]
    public float nGravity; 
    public float fGravity; 
    public float jGravity;

    [Header("Skok dane")]
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpRelease;

    [Header("Czy ziemia pod nogami?")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isGrounded;


    private void Start()
    {
        rb.gravityScale = nGravity;
    }

    void Update()
    {
        Flip();
        HandleAnimations();
    }
    void FixedUpdate()
    {
        CheckGrounded();
        ApplyVariableGravity();
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement() //ruch
    {
        float targetSpeed = moveInput.x * speed; 
        rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
    }
    private void HandleJump() //skok
    {
        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpf);
            jumpPressed = false;
            jumpRelease = false;
        }
        if (jumpRelease)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            }
            jumpRelease = false;
        }
    }
    void HandleAnimations() //animacje
    {
        anim.SetBool("isJumping", rb.velocity.y > .1f);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isIdle", Mathf.Abs(moveInput.x) < .1f && isGrounded);
        anim.SetBool("isWalking", Mathf.Abs(moveInput.x) > .1f && isGrounded);
    }

    void ApplyVariableGravity() //zmienianie grawitacji
    {
        if (rb.velocity.y < -0.1f) 
        {
            rb.gravityScale = fGravity;
        }
        else if (rb.velocity.y > 0.1f) 
        {
            rb.gravityScale = jGravity;
        }
        else
        {
            rb.gravityScale = nGravity;
        }
    }

    void CheckGrounded() //czy ziemia pod nogami
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void Flip() //zmiania kierunku patrzenia
    {
        if (moveInput.x > 0.1f) 
        {
            faceing = 1;
        }
        else if (moveInput.x < -0.1f) 
        {
            faceing = -1;
        }
        transform.localScale = new Vector3(faceing, 1, 1); 
    }
    public void OnMove(InputValue value) //dane wejsciowe ruch
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump (InputValue value) //dane wejsciowe skok
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            jumpRelease = false;
        }
        else 
        {
            jumpRelease = true;
        }
    }

    private void OnDrawGizmosSelected() //wizualizacja sprawdzania czy ziemia jest pod nogami
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
