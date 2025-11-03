using UnityEngine;

public class GunshipController : MonoBehaviour
{
    [SerializeField] private Transform leftCannon;
    [SerializeField] private Transform rightCannon;

    public GameObject cannonballPrefab;
    public float cannonballForce = 150f;

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 leftDirection = AimCannon(mousePosition, leftCannon);
        Vector3 rightDirection = AimCannon(mousePosition, rightCannon);

        FireCannons();
    }

    private Vector3 AimCannon(Vector3 target, Transform cannon)
    {
        Vector3 direction = target - cannon.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cannon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        return direction;
    }

    private void FireCannons()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireCannonball(leftCannon.right, leftCannon);
        }
        if (Input.GetMouseButtonDown(1))
        {
            FireCannonball(rightCannon.right, rightCannon);
        }
    }

    private void FireCannonball(Vector3 direction, Transform cannon)
    {
        GameObject newCannonball = Instantiate(cannonballPrefab, cannon.position, Quaternion.identity);
        newCannonball.GetComponent<Rigidbody2D>().AddForce(direction * cannonballForce);
    }
}
