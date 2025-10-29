using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool isBlocking = false;

    public void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        if (isBlocking)
        {
            amount /= 2;
            Debug.Log($"{name} blocked the attack! Reduced damage: {amount}");
        }

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log($"{name} Health: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    public void SetBlocking(bool blocking) => isBlocking = blocking;

    private void Die()
    {
        Debug.Log($"{name} is defeated!");
        var anim = GetComponentInChildren<Animator>();
        if (anim) anim.SetTrigger("Defeat");
    }
}
