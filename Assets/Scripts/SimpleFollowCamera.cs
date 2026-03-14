using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + target.rotation * offset;
        transform.LookAt(target);
    }
}
