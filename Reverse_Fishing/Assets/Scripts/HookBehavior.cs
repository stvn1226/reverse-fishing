using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    public float fallSpeed = 10.0f; // Initial falling speed
    public float slowDownRate = 2.0f; // Rate at which the object slows down
    public TextMeshPro powDisplay;


    private GameObject anchor; // The anchor point at the top
    private Rigidbody2D rb;
    private Collider2D collider;
    private SpriteRenderer sprite;


    public GameObject Anchor { set { anchor = value; } }


    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        collider= rb.GetComponent<Collider2D>();
        rb.velocity = Vector2.down * fallSpeed;
    }

    private void Update()
    {
        // Calculate the distance between the object and the anchor
        float distance = Vector2.Distance(transform.position, anchor.transform.position);

        // Gradually slow down the object as it descends
        //rb.velocity = Vector2.down * (fallSpeed - distance * slowDownRate);

        // When the object reaches the water surface, you can add additional behavior, e.g., splash effects.
        if (distance >= Random.Range(12.0f, 20.0f))
        {
            rb.velocity = Vector2.up * Random.Range(40.0f, 50.0f);

        }

        if (transform.position.y > 10)
        {
            Destroy(gameObject);
        }
        powDisplay.text = anchor.GetComponent<HookAnchorBehavior>().PowerLevel.ToString();

    }

    void OnTriggerEnter2D(Collider2D fish)
    {
        if (fish.gameObject.tag == "Player")
        {
            fish.gameObject.GetComponent<PlayerController>().Hooked += 1;
            fish.gameObject.GetComponent<PlayerController>().hooks.Add(anchor);
            anchor.GetComponent<HookAnchorBehavior>().currentHook = fish.gameObject;
            //transform.position = fish.transform.position;
            //sprite.enabled = false;
            Destroy(gameObject);
        }
    }

}
