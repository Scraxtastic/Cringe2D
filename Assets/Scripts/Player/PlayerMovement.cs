using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float jumpForce = 35;
    [SerializeField] private float maxSpeedX = 13f;
    [SerializeField] private float maxFallspeed = 25f;
    [SerializeField] private float keepSpeedValue = 0.99f;
    [SerializeField] private float gravityScale = 10;
    [SerializeField] private bool hasJump = false;
    [SerializeField] private bool hasDoubleJump = false;
    [SerializeField] private bool hasDash = false;
    [SerializeField] private float dashDistance = 5;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float groundedSpeedDelta = .001f;
    [SerializeField] private bool dashDeniesGravity = true;
    [SerializeField] private bool dashDeniesJump = true;
    [SerializeField] private bool dashDeniesDoubleJump = true;
    [SerializeField] private bool dashDeniesYVelocity = true;
    [SerializeField] private bool dashDeniesXVelocity = true;
    [SerializeField] private bool dashDeniesXAcceleration = true;

    private PlayerController playerControls;
    private PlayerController.MovementActions movement;
    private Rigidbody2D rigidbody;
    private SpriteRenderer sprite;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool usedDoubleJump = false;
    private bool isDashing = false;
    private float lastDashTime = 0;
    private float dashStartTime = 0;
    private bool dashLeft = true;
    private bool usedDash = false;

    private float keepSpeed = 1;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = gravityScale;
        PlayerGroundedHandler[] handlers = GetComponentsInChildren<PlayerGroundedHandler>();
        foreach (PlayerGroundedHandler handler in handlers)
        {
            handler.maxHeight = 0.015f;
            handler.OnGrounded += OnGrounded;
        }
    }

    private void OnEnable()
    {
        playerControls = new PlayerController();
        playerControls.Enable();
        movement = playerControls.Movement;
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        playerControls.Disable();
    }

    private void Update()
    {
        AddMoveSpeed();
        LimitSpeed();
        CheckJump();
        if (rigidbody.velocity.y <= 0)
        {
            isJumping = false;
        }

        if (Mathf.Abs(rigidbody.velocity.y) > groundedSpeedDelta)
        {
            isGrounded = false;
        }
        CheckDash();
        Dash();
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
        //if objects can move player, the dash shouldn't set the x velocity (might cause problems)
        if (!isDashing && dashDeniesXAcceleration)
            rigidbody.velocity = vel + speedVec;// * Time.deltaTime;
        if (speedVec.x > 0)
        {
            sprite.flipX = false;
        }
        else if (speedVec.x < 0)
        {
            sprite.flipX = true;
        }
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
        if (rigidbody.velocity.y < -maxFallspeed)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -maxFallspeed);
        }
    }

    private void CheckJump()
    {
        if (isDashing && dashDeniesJump) return;
        if (!hasJump) return;
        Vector3 now = transform.position;
        if (isGrounded)
        {
            if (isJumping) return;
            if (movement.Jump.ReadValue<float>() < 0.5f) return;
            Jump();
            usedDoubleJump = false;
        }
        else
        {
            Debug.DrawLine(now, new Vector3(now.x + 5, now.y), Color.red);
            CheckDoubleJump();
        }
    }

    private void CheckDoubleJump()
    {
        if (isDashing && dashDeniesDoubleJump) return;
        if (!movement.Jump.triggered || !movement.Jump.IsPressed()) return;
        if (!hasDoubleJump) return;
        if (usedDoubleJump) return;
        usedDoubleJump = true;
        Jump();
    }
    private void Jump()
    {
        rigidbody.gravityScale = gravityScale;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0);
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        isJumping = true;
    }

    private void CheckDash()
    {
        if (!hasDash) return;
        if (usedDash) return;
        if (isDashing) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        if (!movement.Dash.triggered) return;
        if (movement.Dash.ReadValue<float>() < 0.5f) return;
        StartDash();
    }

    private void StartDash()
    {
        isDashing = true;
        usedDash = true;
        dashStartTime = Time.time;
        dashLeft = sprite.flipX;
        if (dashDeniesGravity)
            rigidbody.gravityScale = 0;
        if (dashDeniesYVelocity)
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
        if (dashDeniesXVelocity)
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    private void Dash()
    {
        if (!isDashing) return;
        if (Time.time > dashStartTime + dashDuration)
        {
            rigidbody.gravityScale = gravityScale;
            isDashing = false;
            return;
        }
        float dashSpeed = dashDistance / dashDuration;
        Vector2 speed = new Vector3();
        if (dashLeft)
        {
            speed.x -= dashSpeed;
        }
        else
        {
            speed.x += dashSpeed;
        }
        rigidbody.velocity += speed;
        lastDashTime = Time.time;
    }

    private void OnGrounded()
    {
        isGrounded = true;
        if (rigidbody.velocity.y > groundedSpeedDelta) return;
        usedDoubleJump = false;
        usedDash = false;
    }
}
