using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Vector3 position1;
    public Transform position2;

    public float speed = 1;

    public float minimumDistance = 0.1f;

    Vector3 target;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        position1 = transform.position;
        target = position2.position;
        rb.linearVelocity = (target - transform.position).normalized * speed;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(position1, position2.position);

        if (Vector2.Distance(transform.position, target) < minimumDistance)
        {
            target = target == position1? position2.position : position1;
            rb.linearVelocity = (target - transform.position).normalized * speed;
        }
    }
}
