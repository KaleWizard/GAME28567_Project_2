using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class TilemapCamera2D : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 5f;
    [SerializeField] Tilemap tilemap;

    Camera followCamera;
    Vector3 offset;
    Vector2 viewportHalfSize;

    float leftCameraBound;
    float rightCameraBound;
    float topCameraBound;
    float bottomCameraBound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        followCamera = GetComponent<Camera>();

        offset = transform.position - target.position;

        tilemap.CompressBounds();

        CalculateCameraBounds();
    }

    private void CalculateCameraBounds()
    {
        float orthographicSize = followCamera.orthographicSize;
        viewportHalfSize = new Vector2(orthographicSize * followCamera.aspect, orthographicSize);

        Vector3Int tilemapMin = tilemap.cellBounds.min;
        Vector3Int tilemapMax = tilemap.cellBounds.max;

        topCameraBound = tilemapMax.y - viewportHalfSize.y;
        bottomCameraBound = tilemapMin.y + viewportHalfSize.y;


        rightCameraBound = tilemapMax.x - viewportHalfSize.x;
        leftCameraBound = tilemapMin.x + viewportHalfSize.x;
    }

    private void LateUpdate()
    {
        Vector2 desiredPosition = target.position + offset;

        Vector3 steppedposition = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);

        steppedposition.x = Mathf.Clamp(steppedposition.x, leftCameraBound, rightCameraBound);
        steppedposition.y = Mathf.Clamp(steppedposition.y, bottomCameraBound, topCameraBound);

        steppedposition.z = -10f;

        transform.position = steppedposition;
    }
}
