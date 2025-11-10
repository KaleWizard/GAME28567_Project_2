using UnityEngine;

public class TypeSwapper : MonoBehaviour
{
    Rigidbody2D rb;

    SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            sr.color = Color.white;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            rb.bodyType = RigidbodyType2D.Static;
            sr.color = Color.black;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            sr.color = Color.red;
        }
    }
}
