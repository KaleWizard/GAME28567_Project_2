using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ContactTesting : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] List<GameObject> points = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        foreach (GameObject p in points)
        {
            p.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject p in points)
        {
            p.SetActive(false);
        }
        List<ContactPoint2D> contacts = new();
        int contactCount = rb.GetContacts(contacts);
        for (int i = 0; i < Mathf.Min(contactCount, points.Count); i++)
        {
            points[i].SetActive(true);
            points[i].transform.position = contacts[i].point;
        }
    }
}
