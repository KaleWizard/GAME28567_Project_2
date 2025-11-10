using UnityEngine;

public class PlayerControllerBasic : MonoBehaviour
{
    Rigidbody2D rb;

    public float moveStrength = 1;
    public float jumpStrength = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocityX = horizontal * moveStrength;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocityY = jumpStrength;
        }
    }
}
