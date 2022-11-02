using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float maxSpeedX = 10f;
    [SerializeField] private float maxSpeedY = 10f;
    [SerializeField] private float maxFallSpeedForJump = 5;

    private PlayerController playerControls;
    private PlayerController.MovementActions movement;
    private Rigidbody2D rigidbody;
    private bool isGrounded = false;
    private bool isJumping = false;    

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        PlayerGroundedHandler[] handlers = GetComponentsInChildren<PlayerGroundedHandler>();
        foreach (PlayerGroundedHandler handler in handlers)
        {
            handler.rb2D = rigidbody;
            handler.OnGrounded += OnGrounded;
        }
    }
    private void Awake()
    {
        playerControls = new PlayerController();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        movement = playerControls.Movement;
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        playerControls.Disable();
    }

    private void FixedUpdate()
    {
        AddMoveSpeed();
        LimitSpeed();
        //rigidbody.position += speedVec * Time.deltaTime;
        if (!isJumping && isGrounded && movement.Jump.ReadValue<float>() > 0.5f)
        {
            Jump();
        }
        if(rigidbody.velocity.y < 0)
        {
            if (isJumping)
            {
                isJumping = false;
            }
            isGrounded = false;
        }
        if (rigidbody.velocity.y < maxFallSpeedForJump)
        {
            isJumping = true;
        }
    }

    private void AddMoveSpeed()
    {
        Vector2 speedVec = new Vector2();
        speedVec.x -= speed * movement.Left.ReadValue<float>();
        speedVec.x += speed * movement.Right.ReadValue<float>();
        rigidbody.velocity += speedVec * Time.deltaTime;
    }

    private void LimitSpeed()
    {
        if (rigidbody.velocity.x > maxSpeedX)
        {
            rigidbody.velocity = new Vector2(maxSpeedX, rigidbody.velocity.y);
        }
        else if (rigidbody.velocity.x < -maxSpeedX)
        {
            rigidbody.velocity = new Vector2(-maxSpeedX, rigidbody.velocity.y);
        }
        if (rigidbody.velocity.y < -maxSpeedY)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -maxSpeedY);
        }
    }
    private void Jump()
    {
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        isJumping = true;
    }

    private void OnGrounded()
    {
        isGrounded = true;
        isJumping = false;
    }
}
