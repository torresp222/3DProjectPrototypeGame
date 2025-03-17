using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoulCombatAndStats : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    // Base stats modified by boosts
    public float baseAttack = 20f;
    public float baseDefense = 15f;

    // Current modified stats
    public float currentAttack;
    [HideInInspector] public float currentDefense;

    // Boost durations
    private float attackBoostDuration = 5f;
    private float defenseBoostDuration = 5f;

    private void Awake() {
        currentHealth = maxHealth;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
    }

    public void TakeDamage(float damage) {
        float mitigatedDamage = damage - (currentDefense * 0.3f);
        currentHealth -= Mathf.Max(mitigatedDamage, 5f);

        if (currentHealth <= 0) Die();
    }

    public void ApplyAttackBoost(float multiplier) {
        currentAttack = baseAttack * multiplier;
        CancelInvoke(nameof(ResetAttack));
        Invoke(nameof(ResetAttack), attackBoostDuration);
    }

    public void ApplyDefenseBoost(float multiplier) {
        currentDefense = baseDefense * multiplier;
        CancelInvoke(nameof(ResetDefense));
        Invoke(nameof(ResetDefense), defenseBoostDuration);
    }

    private void ResetAttack() => currentAttack = baseAttack;
    private void ResetDefense() => currentDefense = baseDefense;

    private void Die() {
        // Handle death logic
    }
}
