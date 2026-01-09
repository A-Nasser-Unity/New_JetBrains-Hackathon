using UnityEngine;

public class MoveUpAndDestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    void Update()
    {
        transform.position += new Vector3(0, 0.02f, 0);
    }
}