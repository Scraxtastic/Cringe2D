using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerGroundedHandler : MonoBehaviour
{
    [SerializeField] private string[] tagsForActivation = new string[] { "Map" };
    [SerializeField] public float maxHeight;
    [HideInInspector] public Rigidbody2D rigidbody;
    public event Action OnGrounded;
    protected void InvokeOnGrounded()
    {
        OnGrounded?.Invoke();
    }

    void FixedUpdate()
    {
        if (rigidbody == null) return;
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);

        // If it hits something...
        if (hit.collider != null)
        {
            Debug.Log("Hit something");
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            if (distance < maxHeight)
            {
                InvokeOnGrounded();
            }
        }
    }

}
