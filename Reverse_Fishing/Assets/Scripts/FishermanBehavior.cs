using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class FishermanBehavior : MonoBehaviour
{
    AudioSource audioData;
    private int powerLevel = 0;
    private float floatForce = 5.0f; // Adjust this to control the buoyancy force.
    private int fallForce = 9;
    private Rigidbody2D rb;

    public int PowerLevel
    {
        get { return powerLevel; }
        set { powerLevel = value; }
    }

    void Start()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play(0);
        rb = GetComponent<Rigidbody2D>();
        int rng = Random.Range(fallForce, (fallForce + 3));
        rb.AddForce(Vector2.down * rng * 100, ForceMode2D.Force); // Set an initial downward force.
    }

    void Update()
    {
        // Apply an upward force to simulate buoyancy.
        rb.AddForce(Vector2.up * floatForce, ForceMode2D.Force);

        if (transform.position.y > 10)
        {
            Destroy(gameObject);
        }
    }
}
