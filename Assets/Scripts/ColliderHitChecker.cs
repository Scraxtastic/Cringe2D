using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColliderHitChecker : MonoBehaviour
{

    enum Directions { Up, Down, Left, Right };
    [SerializeField] private Directions direction = Directions.Down;
    [SerializeField] private string[] tagsForActivation = new string[] { "Map" };
    [SerializeField] private int layerMaskId = 0;
    [SerializeField] public float maxHeight;
    [SerializeField] public bool shallNotCollideWithItself = false;
    public event Action OnHit;
    public event Action OnHitExit;

    private bool initial = true;
    private bool state = true;
    protected void InvokeOnHit()
    {
        OnHit?.Invoke();
    }

    protected void InvokeOnHitExit()
    {
        OnHitExit?.Invoke();
    }

    void Update()
    {
        Vector2 dir = GetDirection();
        // Cast a ray straight into the destinated direction.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxHeight, layerMaskId);
        // If it hits something...
        if (hit.collider != null)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(hit.point.y - transform.position.y, 2) + Mathf.Pow(hit.point.x - transform.position.x, 2));
            if (distance > maxHeight)
            {
                Vector2 added = dir * maxHeight;
                Vector3 maxPosition = transform.position + new Vector3(added.x, added.y, 0);
                Debug.DrawLine(transform.position, maxPosition, Color.yellow);
                return;
            }
            foreach (string tag in tagsForActivation)
            {
                if (hit.collider.gameObject.CompareTag(tag))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                    InvokeOnGrounded();
                    return;
                }
            }
            Debug.DrawLine(transform.position, hit.point, Color.yellow);

        }
    }

    private Vector2 GetDirection()
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
        return dir;
    }
}
