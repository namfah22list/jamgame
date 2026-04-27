using UnityEngine;

public class CursorToggle : MonoBehaviour
{
    private bool isCursorOn = false;

    void Start()
    {
        LockCursor(); // เริ่มเกม = ล็อกเมาส์
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (isCursorOn)
                LockCursor();
            else
                UnlockCursor();
        }
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorOn = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorOn = false;
    }
}