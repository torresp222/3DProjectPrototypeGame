using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Combat }
public class SoulSpirithar : Spirithar {

    [Header("Combat Shoot Settings")]
    [SerializeField] private GameObject _projectilePrefab;

    [SerializeField] private EnemyState _currentState = EnemyState.Idle;
    public EnemyState CurrentStateMode {
        get => _currentState;
        set => _currentState = value;
    }
    private void Awake() {
        elementType = ElementType.Fire;
        currentHealth = maxHealth;
        SetInitialState();
    }

    private void SetInitialState() {
        CurrentCombatMode = CombatMode.Soul;
        _currentState = EnemyState.Idle;
    }

    private void OnEnable() {
        CaptureBall.OnSpiritharSoulCaptured += StartCombatBehavior;
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharSoulCaptured -= StartCombatBehavior;
    }

    private void StartCombatBehavior() {
        _currentState = EnemyState.Combat;
    }

    public void ShootProjectile(Transform objective) {
        Vector3 shootingPosition = transform.position /*+ Vector3.up*/;
        GameObject projectile = Instantiate(_projectilePrefab, shootingPosition, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddForce(transform.forward * 32f, ForceMode.Impulse);
        projectile.GetComponent<Rigidbody>().AddForce(transform.up * 8f, ForceMode.Impulse);
        Destroy(projectile, 3f);
    }

    public override void PerformMove(SpiritharMove spiritharMove, Spirithar target) {
        Debug.LogWarning("PerformMove not used in Soul Combat mode");
    }

    public override bool IsWeak(SpiritharMove spiritharMove) {
        return spiritharMove.moveElementType == ElementType.Water;
    }

}