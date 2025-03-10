using System;
using System.Collections;
using UnityEngine;

public enum ElementType { Fire, Water, Plant, Earth, None }

public abstract class Spirithar : MonoBehaviour {
    public string spiritharName;
    public float maxHealth;
    public int Lvl;
    [HideInInspector]
    public float currentHealth;
    public ElementType elementType;
    public bool PerformingMove;
    // Se espera que se asignen 4 movimientos en el editor
    public SpiritharMove[] moves;

    // Lista de posibles estadísticas base para este Spirithar.
    public SpiritharBaseStats[] possibleBaseStats;
    // El SO seleccionado.
    public SpiritharBaseStats baseStats;
    // Estadísticas en tiempo de juego.
    public SpiritharStats stats;

    // Actions Events
    public static event Action OnTakeDamage;
    public static event Action OnEnemySpiritharNotDead;
    public static event Action OnSpiritharDead;

    public virtual void Initialize() {
        // Seleccionar aleatoriamente un SO de la lista (si hay alguno)
        if (possibleBaseStats != null && possibleBaseStats.Length > 0) {
            int randomIndex = UnityEngine.Random.Range(0, possibleBaseStats.Length);
            baseStats = possibleBaseStats[randomIndex];
        } else {
            Debug.LogWarning("No hay posibles baseStats asignados en " + spiritharName);
        }

        // Inicializamos las estadísticas a partir del SO seleccionado.
        if (baseStats != null) {
            stats = new SpiritharStats(baseStats);
            maxHealth = baseStats.baseHealth;
            currentHealth = maxHealth;
        }
        PerformingMove = false;
    }

    // Método abstracto para realizar un ataque.
    // moveIndex: 0 y 1 para ataques, 2 para defensa, 3 para potenciar ataque.
    public abstract void PerformMove(SpiritharMove spiritharMove, Spirithar target);
    public abstract bool IsWeak(SpiritharMove spiritharMove);

    public IEnumerator SpiritharAttack(Spirithar target, SpiritharMove spiritharMove) {
        bool isWeak = target.IsWeak(spiritharMove);
        float damage = spiritharMove.power;
        // Modificador de ataque del atacante
        float attackerBonus = this.stats.currentAttack * 0.5f;

        // Mitigación de la defensa del defensor
        float defenseReduction = target.stats.currentDefense * 0.3f;

        // Calcular daño total
        damage = damage + attackerBonus - defenseReduction;

        if (isWeak)
            damage = damage * 1.2f;

        // Asegurarse de que el daño mínimo es, por ejemplo, 1
        damage = Mathf.Max(damage, 20f);

        Debug.Log("El DAMAGE RECIBIDO ES " + damage);
        bool isDead = target.TakeDamage(damage);

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
        // Lógica de defensa (por ejemplo, aumentar temporalmente la defensa)
        Debug.Log(spiritharName + " se defiende.");
        OnEnemySpiritharNotDead?.Invoke();
    }

    public virtual void BoostAttack() {
        // Lógica para potenciar el ataque (por ejemplo, aumentar el daño del siguiente ataque)
        Debug.Log(spiritharName + " potencia su ataque.");
        OnEnemySpiritharNotDead?.Invoke();
    }

    protected virtual void Die() {
        Debug.Log(spiritharName + " ha sido derrotado.");
        Destroy(this.gameObject);
        OnSpiritharDead?.Invoke();
        // Aquí podrías destruir el objeto o activar alguna animación
    }

    public bool TakeDamage(float damage) {
        currentHealth -= damage;
        OnTakeDamage?.Invoke();
        if (currentHealth <= 0) { return true; } else { return false; }
    }

}
