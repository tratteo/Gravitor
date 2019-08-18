using UnityEngine;

/// <summary>
/// Attach this component to a gameObject and specify a target transform, the gameObject with this script attached will follow the target transform
/// </summary>
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
