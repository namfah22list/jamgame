using UnityEngine;

public class WigglyFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 0.5f;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 dir = (transform.position - target.position).normalized;
        Vector3 targetPos = target.position + dir * distance;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        transform.LookAt(target);
    }
}