using TMPro;
using UnityEngine;

public class TextFollow : MonoBehaviour
{
    RectTransform rt;

    public TextMeshProUGUI textMesh;

    public Transform following;
    public Vector3 offset = Vector3.up * 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rt.position = following.position + offset;
    }
}
