using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(TopDownPlayerController))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    private bool isDead;

    [Header("UI")]
    public Image healthBarImage;

    [Header("Game Over")]
    [Tooltip("Assign the GameOver manager here")]
    public GameOver gameOverController;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // initialize image
        if (healthBarImage != null)
            healthBarImage.fillAmount = 1f;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // update the fill
        if (healthBarImage != null)
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // update the UI fill
        if (healthBarImage != null)
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;

        Debug.Log($"Player healed by {amount}, now at {currentHealth}/{maxHealth}");
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<TopDownPlayerController>()?.Die();
        Debug.Log("Player died!");

        // SHOW GAME OVER
        if (gameOverController != null)
        {
            gameOverController.gameOver();
        }
        else
        {
            Debug.LogWarning("GameOverController not assigned on PlayerHealth!");
        }
    }

    public bool IsDead() => isDead;
}