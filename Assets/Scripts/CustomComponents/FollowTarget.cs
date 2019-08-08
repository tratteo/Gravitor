using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Start()
    {
        transform.position = target.position + offset;
    }

    void Update()
    {
        transform.position = target.position + offset;
    }
}
