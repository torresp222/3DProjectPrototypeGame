using System;
using System.Collections;
using UnityEngine;

public enum ElementType { Fire, Water, Plant, Earth }

public abstract class Spirithar : MonoBehaviour {
    public string spiritharName;
    public int maxHealth;
    public int Lvl;
    [HideInInspector]
    public int currentHealth;
    public ElementType elementType;
    public bool PerformingMove;
    // Se espera que se asignen 4 movimientos en el editor
    public SpiritharMove[] moves;

    // Actions Events
    public static event Action OnTakeDamage;
    public static event Action OnEnemySpiritharNotDead;
    public static event Action OnSpiritharDead;

    public virtual void Initialize() {
        currentHealth = maxHealth;
        PerformingMove = false;
    }

    // M�todo abstracto para realizar un ataque.
    // moveIndex: 0 y 1 para ataques, 2 para defensa, 3 para potenciar ataque.
    public abstract void PerformMove(SpiritharMove spiritharMove, Spirithar target);

    public virtual void ReceiveDamage(int amount) {
        currentHealth -= amount;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public IEnumerator SpiritharAttack(Spirithar target, SpiritharMove spiritharMove) {
        bool isDead = target.TakeDamage(spiritharMove.power);

        yield return new WaitForSeconds(2f);

        if (isDead) {
            target.Die();
            print("Algo");
        } else {
            print("Change State");
            OnEnemySpiritharNotDead?.Invoke();
        }

    }

    public virtual void Defend() {
        // L�gica de defensa (por ejemplo, aumentar temporalmente la defensa)
        Debug.Log(spiritharName + " se defiende.");
        OnEnemySpiritharNotDead?.Invoke();
    }

    public virtual void BoostAttack() {
        // L�gica para potenciar el ataque (por ejemplo, aumentar el da�o del siguiente ataque)
        Debug.Log(spiritharName + " potencia su ataque.");
        OnEnemySpiritharNotDead?.Invoke();
    }

    protected virtual void Die() {
        Debug.Log(spiritharName + " ha sido derrotado.");
        Destroy(this.gameObject);
        OnSpiritharDead?.Invoke();
        // Aqu� podr�as destruir el objeto o activar alguna animaci�n
    }

    public bool TakeDamage(int damage) {
        currentHealth -= damage;
        OnTakeDamage?.Invoke();
        if (currentHealth <= 0) { return true; } else { return false; }
    }

}
