using TreeEditor;
using UnityEngine;
using UnityEngine.AI;


public class SoulSpirithar : Spirithar {

    [Header("Combat Shoot Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private int _shootForwardForce;
    [SerializeField] private int _shootUpForce;

    private void Awake() {
        elementType = ElementType.Fire;
        currentHealth = maxHealth;
        SetInitialState();
    }

    private void SetInitialState() {
        CurrentCombatMode = CombatMode.Soul;
        _currentState = EnemyState.Idle;
    }

   /* private void OnEnable() {
        CaptureBall.OnSpiritharSoulCaptured += StartCombatBehavior;
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharSoulCaptured -= StartCombatBehavior;
    }

    private void StartCombatBehavior(Spirithar spirithar) {
        _currentState = EnemyState.Combat;
    }*/

    public void ShootProjectile(Transform objective) {
        print("SHOOOOT PROJECTILEEEE");
        //Vector3 shootingPosition = transform.position /*+ Vector3.up*/;
        GameObject projectile = Instantiate(this.moves[0].SoulProjectilePrefab, AttackLaunch.position, Quaternion.identity);
        SoulProjectile soulProjectile = projectile.GetComponent<SoulProjectile>();
        soulProjectile.Initialize(this.moves[0], transform, _shootForwardForce, _shootUpForce);
        /*projectile.GetComponent<Rigidbody>().AddForce(transform.forward * _shootForwardForce, ForceMode.Impulse);
        projectile.GetComponent<Rigidbody>().AddForce(transform.up * _shootUpForce, ForceMode.Impulse);*/
        
    }

    public override void PerformMove(SpiritharMove spiritharMove, Spirithar target, bool isSpiritharFromTeam = false) {
        Debug.LogWarning("PerformMove not used in Soul Combat mode");
    }

    public override bool IsWeak(SpiritharMove spiritharMove) {
        return spiritharMove.moveElementType == ElementType.Water;
    }

}