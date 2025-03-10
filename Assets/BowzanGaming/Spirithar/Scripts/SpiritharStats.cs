using UnityEngine;


[System.Serializable]
public class SpiritharStats {
    public float currentHealth;
    public float currentAttack;
    public float currentDefense;

    // Inicializa con los valores base.
    public SpiritharStats(SpiritharBaseStats baseStats) {
        currentHealth = baseStats.baseHealth;
        currentAttack = baseStats.baseAttack;
        currentDefense = baseStats.baseDefense;
    }

    // Método para resetear (si es necesario)
    public void Reset(SpiritharBaseStats baseStats) {
        currentHealth = baseStats.baseHealth;
        currentAttack = baseStats.baseAttack;
        currentDefense = baseStats.baseDefense;
    }
}
