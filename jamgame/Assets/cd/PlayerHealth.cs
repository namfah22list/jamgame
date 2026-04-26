using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP Settings")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("UI (ไม่บังคับ)")]
    public Slider hpSlider;        // ลาก Slider HP มาใส่ (ถ้ามี)
    public TextMeshProUGUI hpText; // ลาก Text มาใส่ (ถ้ามี)

    public bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        Debug.Log($"Player โดนตี! HP: {currentHP}/{maxHP}");

        UpdateUI();

        if (currentHP <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        UpdateUI();
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player ตายแล้ว! Game Over");
        // ใส่ logic Game Over ตรงนี้
        // เช่น SceneManager.LoadScene("GameOver");
    }

    void UpdateUI()
    {
        if (hpSlider) hpSlider.value = currentHP / maxHP;
        if (hpText) hpText.text = $"HP: {currentHP:F0}/{maxHP}";
    }
}