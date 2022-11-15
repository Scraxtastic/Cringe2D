using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDrop : MonoBehaviour
{
    [SerializeField] private float dropDelay = 0f;
    [SerializeField] private float dropSpeed = 1f;
    [SerializeField] private float range = 100;

    private bool isDropping = false;
    private float dropStart = float.MaxValue;
    private void Start()
    {
        PlayerGroundedHandler[] handlers = GetComponentsInChildren<PlayerGroundedHandler>();
        foreach (PlayerGroundedHandler handler in handlers)
        {
            handler.maxHeight = range;
            handler.OnGrounded += OnGrounded;
        }
    }

    public void OnGrounded()
    {
        if (isDropping) return;
        isDropping = true;
        dropStart = Time.time + dropDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > dropStart)
        {
            Drop();
        }
    }

    public void Drop()
    {
        Vector3 pos = transform.position;
        pos.y -= dropSpeed * Time.deltaTime;
        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(this.gameObject);
    }
}
