using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDrop : MonoBehaviour
{
    [SerializeField] private float dropDelay = 0f;
    [SerializeField] private float dropSpeed = 1f;
    [SerializeField] private float range = 100;
    private Rigidbody2D rigidbody;

    private bool isDropping = false;
    private float dropStart = float.MaxValue;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        PlayerGroundedHandler[] handlers = GetComponentsInChildren<PlayerGroundedHandler>();
        foreach (PlayerGroundedHandler handler in handlers)
        {
            handler.rigidbody = rigidbody;
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
        rigidbody.gravityScale = dropSpeed;
        dropStart = float.MaxValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Currently just destroys the spike (further mechanics not yet known :c)");
            Destroy(this.gameObject);
        }
    }
}
