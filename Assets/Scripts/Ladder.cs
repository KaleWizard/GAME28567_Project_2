using UnityEngine;

public class Ladder : MonoBehaviour
{
    void Start()
    {
        LadderRegister.Instance.ladderColliders.Add(GetComponent<BoxCollider2D>());
    }
}
