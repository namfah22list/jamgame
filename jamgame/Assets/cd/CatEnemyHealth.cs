using UnityEngine;

public class CatEnemyHealth : MonoBehaviour
{
    public float maxHP = 50f;
    public float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log($"แมวโดนตี! HP: {currentHP}/{maxHP}");

        if (currentHP <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("แมวตายแล้ว!");
        // ใส่ animation ตาย หรือ Destroy ได้เลย
        Destroy(gameObject, 0.5f);
    }
}