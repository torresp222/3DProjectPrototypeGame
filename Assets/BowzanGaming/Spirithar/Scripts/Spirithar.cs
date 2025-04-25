using System;
using System.Collections;
using UnityEngine;

public enum ElementType { Fire, Water, Plant, Earth, None }
public enum CombatMode { TurnBased, Soul }

public enum EnemyState { Idle, Combat }

public abstract class Spirithar : MonoBehaviour {
    [Header("Combat Settings")]
    [SerializeField] private CombatMode _combatMode = CombatMode.TurnBased;
    [SerializeField] public Transform AttackLaunch;
    [SerializeField] protected EnemyState _currentState = EnemyState.Idle;

    public EnemyState CurrentStateMode {
        get => _currentState;
        set => _currentState = value;
    }
    /// <summary>
    /// Current combat mode determining behavior logic
    /// </summary>
    public CombatMode CurrentCombatMode {
        get => _combatMode;
        set => _combatMode = value;
    }

    public string spiritharName;
    public float maxHealth;
    public int Lvl;
    /*[HideInInspector]*/
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
    public bool BaseStatsDone = false;

    // Actions Events
    public static event Action<CombatMode> OnTakeDamage;
   // public static event Action OnEnemySpiritharNotDead;
    public static event Action OnSpiritharDead;

    public virtual void Initialize() {
       
        PerformingMove = false;
    }

    public void SetFirstStats() {
        // Seleccionar aleatoriamente un SO de la lista (si hay alguno)
        if (possibleBaseStats != null && possibleBaseStats.Length > 0) {
            int randomIndex = UnityEngine.Random.Range(0, possibleBaseStats.Length);
            baseStats = possibleBaseStats[randomIndex];

        } else {
            Debug.LogWarning("No hay posibles baseStats asignados en " + spiritharName);
        }

        // Inicializamos las estadísticas a partir del SO seleccionado.
        if (baseStats != null && !BaseStatsDone) {
            stats = new SpiritharStats(baseStats);
            maxHealth = baseStats.baseHealth;
            currentHealth = maxHealth;
            BaseStatsDone = true;

            Debug.Log("Entramos aquí en el inicializar del spirithar " + this.spiritharName);
        }
    }
    public SpiritharBaseStats SetGetFirstStats() {
        // Seleccionar aleatoriamente un SO de la lista (si hay alguno)
        if (possibleBaseStats != null && possibleBaseStats.Length > 0) {
            int randomIndex = UnityEngine.Random.Range(0, possibleBaseStats.Length);
            baseStats = possibleBaseStats[randomIndex];
            
        } else {
            Debug.LogWarning("No hay posibles baseStats asignados en " + spiritharName);
        }

        // Inicializamos las estadísticas a partir del SO seleccionado.
        if (baseStats != null && !BaseStatsDone) {
            stats = new SpiritharStats(baseStats);
            maxHealth = baseStats.baseHealth;
            currentHealth = maxHealth;
            BaseStatsDone = true;
            
            Debug.Log("Entramos aquí en el inicializar del spirithar " + this.spiritharName);
        }

        return baseStats;
    }

    // Método abstracto para realizar un ataque.
    // moveIndex: 0 y 1 para ataques, 2 para defensa, 3 para potenciar ataque.
    public abstract void PerformMove(SpiritharMove spiritharMove, Spirithar target, bool isSpiritharFromTeam = false);
    public abstract bool IsWeak(SpiritharMove spiritharMove);

    public IEnumerator SpiritharAttack(Spirithar target, SpiritharMove spiritharMove, bool isSpiritharFromTeam) {
        bool isWeak = target.IsWeak(spiritharMove);
        float damage = spiritharMove.power;
        // Modificador de ataque del atacante
        float attackerBonus = this.stats.currentAttack * 0.5f;

        // Mitigación de la defensa del defensor
        float defenseReduction = target.stats.currentDefense * 0.3f;
        Debug.Log("Defensa reductiooon " + defenseReduction);

        // Calcular daño total
        damage = damage + attackerBonus - defenseReduction;

        if (isWeak)
            damage = damage * 1.2f;

        // Asegurarse de que el daño mínimo es, por ejemplo, 1
        damage = Mathf.Max(damage, 20f);

        GameObject projectile = Instantiate(spiritharMove.projectilePrefab, this.AttackLaunch.position, Quaternion.identity);
        Projectile projComponent = projectile.GetComponent<Projectile>();
        projComponent.TurnBasedInitialize(target.transform.position);
        

        yield return new WaitForSeconds(1f);
        
        bool isDead = target.TakeDamage(damage);
        Debug.Log("El DAMAGE RECIBIDO ES " + damage);

        if (isDead) {
            if (isSpiritharFromTeam)
                Debug.Log("Spirithar dead is from TEAM");
            else
                Debug.Log("Spirithar dead is ENEMY, not from TEAM");
            target.Die();
            print("DEADDEAD");
        } else {
            print("Change State");
            //OnEnemySpiritharNotDead?.Invoke();
        }

    }

    public virtual IEnumerator Defend(SpiritharMove spiritharMove) {
        float defenseIncrease = 0.5f * spiritharMove.power;
        this.stats.currentDefense += defenseIncrease;
        // Lógica de defensa (por ejemplo, aumentar temporalmente la defensa)
        Debug.Log(spiritharName + " se defiende.");
        yield return new WaitForSeconds(1f);
        print("Change State");
        //OnEnemySpiritharNotDead?.Invoke();
    }

    public virtual IEnumerator BoostAttack(SpiritharMove spiritharMove) {
        float attackIncrease = 0.2f * spiritharMove.power;
        this.stats.currentAttack += attackIncrease;
        // Lógica para potenciar el ataque (por ejemplo, aumentar el daño del siguiente ataque)
        Debug.Log(spiritharName + " potencia su ataque.");
        yield return new WaitForSeconds(1f);
        print("Change State");
        //OnEnemySpiritharNotDead?.Invoke();
    }

    /// <summary>
    /// Handles destruction based on combat mode
    /// </summary>
    public virtual void Die() {
        Debug.Log(spiritharName + " defeated.");

        if (CurrentCombatMode == CombatMode.TurnBased) {
            Destroy(gameObject);
            OnSpiritharDead?.Invoke();
        } else {
            // Soul combat specific cleanup
            SoulCombatManager.Instance?.EndBossCombat();
            Destroy(gameObject);
        }
    }

    /*protected virtual void Die() {
        Debug.Log(spiritharName + " ha sido derrotado.");
        Destroy(this.gameObject);
        OnSpiritharDead?.Invoke();
        // Aquí podrías destruir el objeto o activar alguna animación
    }*/

    public bool TakeDamage(float damage) {
        currentHealth -= damage;
        if (CurrentCombatMode == CombatMode.Soul) {
            print($"{this.spiritharName} ha recibido dañooooo");
        }
        OnTakeDamage?.Invoke(CurrentCombatMode);
        if (currentHealth <= 0) { 
            return true; } else { return false; }
    }

}
