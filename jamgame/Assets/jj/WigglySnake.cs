using System.Collections.Generic;
using UnityEngine;

public class WigglySnake : MonoBehaviour
{
    public Transform[] segments;
    public float segmentSpacing = 0.5f;
    public float moveSpeed = 5f;

    private List<Vector3> path = new List<Vector3>();

    void Start()
    {
        // เติม path เริ่มต้น
        for (int i = 0; i < segments.Length * 10; i++)
        {
            path.Add(transform.position);
        }
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        transform.position += move * moveSpeed * Time.deltaTime;

        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        path.Insert(0, transform.position);

        float distance = 0f;

        for (int i = 0; i < segments.Length; i++)
        {
            distance += segmentSpacing;

            for (int j = 0; j < path.Count - 1; j++)
            {
                float dist = Vector3.Distance(path[j], path[j + 1]);

                if (distance <= dist)
                {
                    segments[i].position = Vector3.Lerp(path[j], path[j + 1], distance / dist);

                    // หมุนตามทาง
                    Vector3 dir = path[j] - path[j + 1];
                    if (dir != Vector3.zero)
                        segments[i].rotation = Quaternion.LookRotation(dir);

                    break;
                }
                else
                {
                    distance -= dist;
                }
            }
        }

        if (path.Count > 500)
            path.RemoveAt(path.Count - 1);
    }
}