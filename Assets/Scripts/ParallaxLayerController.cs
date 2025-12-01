using UnityEngine;

public class ParallaxLayerController : MonoBehaviour
{
    [SerializeField] Camera viewCamera;
    [SerializeField] float cameraDeltaScalar = 1f;

    Vector3 cameraStartPos;
    Vector3 layerStartPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraStartPos = viewCamera.transform.position;
        layerStartPos = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 cameraDelta = viewCamera.transform.position - cameraStartPos; ;

        float layerDeltaX = cameraDelta.x * cameraDeltaScalar;
        float layerDeltaY = cameraDelta.y * cameraDeltaScalar;

        Vector3 newPosition = layerStartPos + new Vector3(layerDeltaX, layerDeltaY);

        transform.position = Vector3.Lerp(transform.position, newPosition, cameraDeltaScalar);
    }
}
