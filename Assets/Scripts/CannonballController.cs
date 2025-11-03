using UnityEngine;

public class CannonballController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CannonballController otherBall = collision.gameObject.GetComponent<CannonballController>();
        if (otherBall == null)
        {
            ScoreboardController.Instance.Score++;
        }
        Destroy(gameObject);
    }
}
