using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HungerSystem : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hunger;

    public float timeToZero = 120f; // 2 นาที
    private float drainRate;

    public Slider hungerBar;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private bool isDead = false;

    void Start()
    {
        // รีเซ็ตเวลาเผื่อมาจากฉากก่อน
        Time.timeScale = 1f;

        // ล็อกเมาส์ตอนเริ่มเกม
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        hunger = maxHunger;
        drainRate = maxHunger / timeToZero;

        if (hungerBar != null)
            hungerBar.value = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        // ลดความหิว
        hunger -= drainRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);

        // อัปเดตหลอด
        if (hungerBar != null)
        {
            hungerBar.value = hunger / maxHunger;
        }

        // เช็คตาย
        if (hunger <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // หยุดเวลาเกม
        Time.timeScale = 0f;

        // ปลดล็อกเมาส์ให้กด UI ได้
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // เปิดหน้าจอ Game Over
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // ฟังก์ชันกิน
    public void Eat(float amount)
    {
        if (isDead) return;

        hunger += amount;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
    }

    // ปุ่ม Restart
    public void RestartGame()
    {
        Time.timeScale = 1f;

        // ล็อกเมาส์กลับก่อนเข้าเกม
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ปุ่ม Quit
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // ใช้ใน Editor
    }
}