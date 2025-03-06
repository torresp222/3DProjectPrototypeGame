using UnityEngine;

public enum ElementType { Fire, Water, Plant, Earth }

public abstract class Spirithar : MonoBehaviour {
    public string spiritharName;
    public int maxHealth;
    public int Lvl;
    [HideInInspector]
    public int currentHealth;
    public ElementType elementType;

    // Se espera que se asignen 4 movimientos en el editor
    public SpiritharMove[] moves;

    public virtual void Initialize() {
        currentHealth = maxHealth;
    }

    // M�todo abstracto para realizar un ataque.
    // moveIndex: 0 y 1 para ataques, 2 para defensa, 3 para potenciar ataque.
    public abstract void PerformMove(int moveIndex, Spirithar target);

    public virtual void ReceiveDamage(int amount) {
        currentHealth -= amount;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public virtual void Defend() {
        // L�gica de defensa (por ejemplo, aumentar temporalmente la defensa)
        Debug.Log(spiritharName + " se defiende.");
    }

    public virtual void BoostAttack() {
        // L�gica para potenciar el ataque (por ejemplo, aumentar el da�o del siguiente ataque)
        Debug.Log(spiritharName + " potencia su ataque.");
    }

    protected virtual void Die() {
        Debug.Log(spiritharName + " ha sido derrotado.");
        // Aqu� podr�as destruir el objeto o activar alguna animaci�n
    }
}
