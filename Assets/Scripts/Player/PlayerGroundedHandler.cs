using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerGroundedHandler : MonoBehaviour
{
    enum Directions { Up, Down, Left, Right };
    [SerializeField] private Directions direction = Directions.Down;
    [SerializeField] private string[] tagsForActivation = new string[] { "Map" };
    [SerializeField] public float maxHeight;
    [HideInInspector] public Rigidbody2D rigidbody;
    public event Action OnGrounded;
    protected void InvokeOnGrounded()
    {
        OnGrounded?.Invoke();
    }

    void Update()
    {
        Vector2 dir = Vector2.up;
        switch (direction)
        {
            case Directions.Up:
                dir = Vector2.up;
                break;
            case Directions.Down:
                dir = Vector2.down;
                break;
            case Directions.Left:
                dir = Vector2.left;
                break;
            case Directions.Right:
                dir = Vector2.right;
                break;
        }
        if (rigidbody == null) return;
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        Debug.DrawRay(transform.position, dir, Color.yellow);
        // If it hits something...
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            if (distance < maxHeight)
            {
                InvokeOnGrounded();
            }
        }
    }

}
