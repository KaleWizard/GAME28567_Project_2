using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform following;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (following != null)
        {
            transform.position = following.position + Vector3.back * 10;
        }
    }
}
