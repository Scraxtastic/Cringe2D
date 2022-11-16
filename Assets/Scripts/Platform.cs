using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float despawnTime = 3f;
    [SerializeField] private float respawnTime = 10f;
    [SerializeField] private List<string> despawnOnTags = new List<string>();

    private Vector3 startPosition;
    private SpriteRenderer sprite;
    private bool shallDespawn = false;
    private bool shallRespawn = false;
    private float despawnAt = 0f;
    private float colorStrength = 0;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckRespawn();
        CheckDespawn();
    }

    private void CheckRespawn()
    {
        if (!shallRespawn) return;
        if (Time.time < despawnAt + respawnTime) return;
        this.transform.position = startPosition;
        sprite.color = new Color(1, 1, 1);
        colorStrength = 0;
        shallRespawn = false;
    }

    private void CheckDespawn()
    {
        if (!shallDespawn) return;
        colorStrength += Time.deltaTime / despawnTime;
        float otherColors = 1 - colorStrength;
        sprite.color = new Color(1, otherColors, otherColors);
        if (Time.time > despawnAt)
        {
            this.transform.position = new Vector3(9000, 9000, 0);
            shallDespawn = false;
            shallRespawn = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (shallDespawn) return;
        if (!collision.enabled) return;
        if (despawnOnTags.Contains(collision.gameObject.tag))
        {
            shallDespawn = true;
            despawnAt = Time.time + despawnTime;
        }
    }
}
