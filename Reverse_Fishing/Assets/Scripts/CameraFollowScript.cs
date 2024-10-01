using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public GameObject player;

    public float yMax;
    public float yMin;
    public float xMax;
    public float xMin;

    // Update is called once per frame
    void Update()
    {
        // Get the player's position
        Vector3 playerPosition = player.transform.position;

        // Clamp the player's position to stay within the play area
        float clampedX = Mathf.Clamp(playerPosition.x, xMin, xMax);
        float clampedY = Mathf.Clamp(playerPosition.y, yMin, yMax);

        // Set the camera's position to follow the clamped player position
        transform.position = new Vector3(clampedX, clampedY, -110.0f);
    }
}
