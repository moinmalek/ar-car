using UnityEngine;

/// <summary>
/// Applies continuous rotation around the forward axis for idle visuals.
/// </summary>
public sealed class JustRotate : MonoBehaviour
{
    [SerializeField] bool canRotate = true;
    [SerializeField] float speed = 10f;

    void Update()
    {
        if (!canRotate)
            return;
        transform.Rotate(Vector3.forward * (speed * Time.deltaTime));
    }
}
