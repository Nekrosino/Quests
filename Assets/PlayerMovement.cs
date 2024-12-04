using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    float horizontalMove = 0f;
    public float runSpeed = 4f;

    private static PlayerMovement instance;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float jumpForce;

    private Rigidbody2D rb;
    Vector2 moveDirection;
    private bool canJump;
    private bool isGrounded;
    private bool interactPressed;
    private bool submitPressed;

    private void Awake()
    {
        if (instance != null )
        {
            Debug.LogError("Wiecej niz jeden input na scenie");
        }
        instance = this;
    }

    public static PlayerMovement GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        resetJump();
    }

    private void FixedUpdate()
    {
        move();
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if(DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }
    }

    private void Update()
    {
        rb.freezeRotation = true;

   

        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", true);
        }
    }

    private void move()
    {
        rb.velocity = new Vector2(moveDirection.normalized.x * speed, rb.velocity.y);
        if (moveDirection.x > 0) // Porusza siê w prawo
        {
            transform.localScale = new Vector3(1, 1, 1); // Ustaw normalny kierunek
        }
        else if (moveDirection.x < 0) // Porusza siê w lewo
        {
            transform.localScale = new Vector3(-1, 1, 1); // Obróæ w poziomie (flip)
        }

    }

    public void onJump(InputAction.CallbackContext context)
    {
        if(context.performed && isGrounded && canJump)
        {
            canJump = false;
            isGrounded = false;
            
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Invoke(nameof(resetJump), .5f);

        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
        }
        else if (context.canceled)
        {
            interactPressed = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    private void resetJump()
    {
        canJump = true;
      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    public bool GetInteractPressed()
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        }
    }

    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }


}
