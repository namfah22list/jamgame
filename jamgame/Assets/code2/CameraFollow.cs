using UnityEngine;

public class CameraFollowAllInOne : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // ตัวหลัก (Body ของจิ้งจก)

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 5, -8);

    [Header("Smooth")]
    public float smoothSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 200f;
    public float minY = -30f;
    public float maxY = 60f;

    private float rotX = 20f; // มุมก้ม/เงย
    private float rotY = 0f;  // มุมซ้ายขวา

    void Start()
    {
        // ล็อคเมาส์ (optional)
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 🖱️ รับค่าเมาส์
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minY, maxY);

        // 🎯 คำนวณทิศทางกล้อง
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);

        // 📍 ตำแหน่งที่ควรไป
        Vector3 desiredPosition = target.position + rotation * offset;

        // 🎥 เคลื่อนแบบลื่น
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // 👀 หันกล้อง
        transform.LookAt(target.position + Vector3.up * 1.5f);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
offset += offset.normalized * scroll * 5f;
    }
}