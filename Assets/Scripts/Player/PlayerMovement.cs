using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float maxSpeedX = 10f;
    [SerializeField] private float maxSpeedY = 10f;
    [SerializeField] private float maxFallSpeedForJump = 5;
    [SerializeField] private float keepSpeedValue = 0.8f;
    [SerializeField] private float gravityScale = 10;

    private PlayerController playerControls;
    private PlayerController.MovementActions movement;
    private Rigidbody2D rigidbody;
    private SpriteRenderer sprite;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float keepSpeed = 1;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        PlayerGroundedHandler[] handlers = GetComponentsInChildren<PlayerGroundedHandler>();
        foreach (PlayerGroundedHandler handler in handlers)
        {
            Debug.Log("Added Rigidbody to: " + handler.name );
            handler.rigidbody = rigidbody;
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
        float leftValue = movement.Left.ReadValue<float>();
        float rightValue = movement.Right.ReadValue<float>();
        Vector2 speedVec = new Vector2();
        speedVec.x -= acceleration * leftValue;
        speedVec.x += acceleration * rightValue;
        if (speedVec.x != 0)
        {
            keepSpeed = 1;
        }
        else
        {
            keepSpeed *= keepSpeedValue;
            if (keepSpeed < 0.01f)
            {
                keepSpeed = 0;
            }
        }
        Vector2 vel = rigidbody.velocity;
        vel.x *= keepSpeed;
        rigidbody.velocity = vel + speedVec * Time.deltaTime;
        if(speedVec.x > 0)
        {
            sprite.flipX = false;
        }else if(speedVec.x < 0)
        {
            sprite.flipX = true;
        }
        Debug.Log(rigidbody.velocity);
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
        rigidbody.gravityScale = gravityScale;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0);
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
