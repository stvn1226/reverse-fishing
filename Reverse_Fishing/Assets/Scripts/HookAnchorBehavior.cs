using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HookAnchorBehavior : MonoBehaviour
{
    //the hook anchor will be an off-screen object that will act as if it were the fishing rod's tip
    //it will spawn a hook, which will descend into the water and remain tethered to the anchor, which is off-screen
    //the anchor will move around somewhat, and will be attached to the hook via a string of set length
    //once the hook is bit, the anchor will pull up vertically, sometimes with an angle
    //the string will be fully tense once the hook is bitten, as the anchor will pull as hard as it can
    //
    
    public GameObject hookPrefab; //there can only be one hook
    public GameObject currentHook;
    public LineRenderer lineRenderer;
    public GameObject fisherPrefab;

    private int powerLevel;
    private int health;
    private GameObject fisher;
    private GameObject player;
    private GameObject spawner;
    private float nextPullTime;

    private bool hasPulled;
    private float force;

    public int PowerLevel { get { return powerLevel; } }
    public int Health { 
        get { return health; }
        set { health = value; } 
    }

    // Start is called before the first frame update
    void Start()
    {
        force = 1.0f;
        hasPulled = false;
        SetNextPullTime();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        player = GameObject.FindGameObjectWithTag("Player");
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        powerLevel = Random.Range((spawner.GetComponent<HookSpawner>().GlobalPower - (spawner.GetComponent<HookSpawner>().GlobalPower / 5)), (spawner.GetComponent<HookSpawner>().GlobalPower + (spawner.GetComponent<HookSpawner>().GlobalPower / 5)));
        health = powerLevel * 10;

        currentHook = Instantiate(hookPrefab, transform);
        currentHook.GetComponent<HookBehavior>().Anchor = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHook == null || health <= 0)
        {
            Despawn();
        }

        UpdateLineRenderer();


        //Pull Fish
        if (currentHook.GetComponent<PlayerController>() != null && Time.time >= nextPullTime)
        {
            if (force <= 1.0f) { 
                force = 50.0f;
                currentHook.GetComponent<PlayerController>().Health -= powerLevel + Random.Range(powerLevel / 10, powerLevel / 2);
            }

            if (force >= 1.0f)
            {
                // Calculate the direction from the fish to the hook
                Vector3 direction = gameObject.transform.position - currentHook.transform.position;

                direction.Normalize();

                // Calculate the new position for the fish object
                Vector3 newPosition = currentHook.transform.position + direction * force * Time.fixedDeltaTime;

                // Move the fish object towards the mouse position
                currentHook.transform.position = newPosition;

                // Reduce the fish's speed
                force *= 0.9f;

                // Rotate the fish to face the hook
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                currentHook.transform.rotation = Quaternion.Slerp(currentHook.transform.rotation, rotation, currentHook.GetComponent<PlayerController>().RotationSpeed * Time.fixedDeltaTime);
            }

            if (force <= 1.0)
            {
                hasPulled = true;
            }
        }

        if (hasPulled)
        {
            SetNextPullTime();
            hasPulled = false;
        }
    }

    private void SetNextPullTime()
    {
        // Set the time for the next spawn within the specified range
        nextPullTime = Time.time + Random.Range(0.1f, 1.0f);
    }


    void Despawn()
    {
        spawner.GetComponent<HookSpawner>().GlobalPower += powerLevel / 4;

        if (currentHook == null)
        {
            spawner.GetComponent<HookSpawner>().spawnedObjects.Remove(gameObject);
            Destroy(gameObject);
        }

        if (health <= 0)
        {
            //spawn fisherman with equal power level
            spawner.GetComponent<HookSpawner>().spawnedObjects.Remove(gameObject);
            player.GetComponent<PlayerController>().hooks.Remove(gameObject);
            player.GetComponent<PlayerController>().Hooked -= 1;
            fisher = Instantiate(fisherPrefab, transform);
            fisher.GetComponent<FishermanBehavior>().PowerLevel = this.powerLevel;
            fisher.transform.SetParent(null);
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.SetPosition(0, transform.position); // Rod position
        if (currentHook != null)
        {
            lineRenderer.SetPosition(1, currentHook.transform.position); // Hook position
        }
    }
}
