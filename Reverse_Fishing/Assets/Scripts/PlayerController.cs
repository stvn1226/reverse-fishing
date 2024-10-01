using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public AudioSource swimSound;
    public AudioSource eatSound;
    public const float originalSpeed = 1.0f;
    public TextMeshPro powDisplay;
    public List<GameObject> hooks;
    public SceneChangerScript sceneChanger;

    private float followSpeed = originalSpeed; // Adjust this to control the follow speed
    private float rotationSpeed = originalSpeed; // Adjust this to control the rotation speed
    private float reductionSpeed = 4;
    private Transform fish;
    private float powerLevel;
    private int hooked;
    private Rect cameraRect;
    Vector3 bottomLeft;
    Vector3 topRight;
    private SpriteRenderer spriteRenderer;
    Color fatigueColor = Color.white;
    private float health;
    private Rigidbody2D rb;

    public int Hooked {
        get { return hooked; }
        set { hooked = value; } 
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = value; }
    }

    public float PowerLevel { get { return powerLevel; } }

    void Start()
    {
        //swimSound = GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(true);
        hooks = new List<GameObject>();
        fish = transform;
        powerLevel = 100;
        health = powerLevel * 10;
        hooked = 0;

        bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
        cameraRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    void Update()
    {
        bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));

        Clamp();
        if (health < 0)
        {
            gameObject.SetActive(false);
            sceneChanger.ChangeScene("GameOver");
        }
        MoveAndRotate();
        Swim();
        DoDamage();
        Heal();
        DisplayFatigue();
        
        if ((hooked <= 0 && followSpeed > 1.0f) || hooked > 0 && followSpeed > 0.1f)
        {
            ReduceSpeed();
        }

        powDisplay.text = "Power Level: " + powerLevel.ToString();
    }

    void MoveAndRotate()
    {
        // Get the current mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the fish object stays in the 2D plane


        // Calculate the direction from the fish to the mouse
        Vector3 direction = mousePosition - fish.position;

        if (Vector2.Distance(mousePosition, fish.position) < 0.01f) return;

        // Normalize the direction vector to make it a unit vector
        direction.Normalize();

        // Calculate the new position for the fish object
        Vector3 newPosition = fish.position + direction * followSpeed * Time.fixedDeltaTime;

        // Move the fish object towards the mouse position
        fish.position = newPosition;

        // Calculate the rotation angle to face the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the fish object smoothly
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        fish.rotation = Quaternion.Slerp(fish.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);

        float way = mousePosition.x;

        if (way < transform.position.x)
        {
            spriteRenderer.flipY = true;
        }
        else if (way > transform.position.x)
        {
            spriteRenderer.flipY = false;
        }
    }

    void Swim()
    {

        int hooker = hooked;
        if (hooker < 1) hooker = 1;

        //click to swim faster
        if (Input.GetMouseButtonDown(0))
        {
            swimSound.Play(0);
            followSpeed = originalSpeed * (12.0f - hooker);
            rotationSpeed = originalSpeed * 8.0f;
        }
    }

    void ReduceSpeed()
    {
        followSpeed -= Time.fixedDeltaTime * reductionSpeed;
        if (rotationSpeed < originalSpeed) return;
        rotationSpeed -= Time.fixedDeltaTime * reductionSpeed;
    }

    void DoDamage()
    {
        if (hooked < 1) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Transform startPosition = fish;
        if (Input.GetMouseButtonDown(0))
        {
            if (mousePosition.y < startPosition.position.y)
            {
                foreach(GameObject h in hooks)
                {
                    h.GetComponent<HookAnchorBehavior>().Health -= (int)(powerLevel - Random.Range(powerLevel / 10, powerLevel / 2));
                }
            }
        }
    }

    void DisplayFatigue()
    {
        //changes the tint of the fish depending on how fatigued he is
        fatigueColor.b = health / (powerLevel * 10.0f);
        fatigueColor.g = health / (powerLevel * 10.0f);
        spriteRenderer.color= fatigueColor;
    }

    void Clamp()
    {
        if (transform.position.y > topRight.y)
        {
            transform.position = new Vector3(transform.position.x, topRight.y);
        }
        if (transform.position.y < topRight.y - cameraRect.height)
        {
            transform.position = new Vector3(transform.position.x, topRight.y - cameraRect.height);
        }

        if (transform.position.x < bottomLeft.x)
        {
            transform.position = new Vector3(bottomLeft.x, transform.position.y);
        }
        if (transform.position.x > bottomLeft.x + cameraRect.width)
        {
            transform.position = new Vector3(bottomLeft.x + cameraRect.width, transform.position.y);
        }
    }

    public void Heal()
    {
        if (hooked == 0 && health <= powerLevel * 10)
        {
            health += powerLevel / 120;
        }
        if (health > powerLevel * 10)
        {
            health = powerLevel * 10;
        }
        
    }

    void OnTriggerEnter2D(Collider2D fisher)
    {
        if (fisher.gameObject.tag == "Fisher")
        {
            powerLevel += fisher.gameObject.GetComponent<FishermanBehavior>().PowerLevel / 4;
            health += fisher.gameObject.GetComponent<FishermanBehavior>().PowerLevel * 4;
            Destroy(fisher.gameObject);
            eatSound.Play();
        }
    }
}
