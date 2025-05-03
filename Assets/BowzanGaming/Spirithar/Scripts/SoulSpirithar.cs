using UnityEngine;


public class SoulSpirithar : Spirithar {

    [Header("Combat Shoot Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private int _shootForwardForce;
    [SerializeField] private int _shootUpForce;
    [SerializeField] private SoulSpiritharAi _spiritharAi;

    private void Awake() {
        elementType = ElementType.Fire;
        currentHealth = maxHealth;
        SetInitialState();

        _spiritharAi = GetComponent<SoulSpiritharAi>();
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
    public void ShootProjectile(Transform objective) { // El parámetro 'objective' no se usa actualmente aquí
        //print("SHOOOOT PROJECTILEEEE");
        //Vector3 targetPoint;

        if (AttackLaunch == null) {
            Debug.LogError("AttackLaunch no está asignado en el Spirithar!", this);
            return;
        }
        if (this.moves == null || this.moves.Length == 0 || this.moves[0].SoulProjectilePrefab == null) {
            Debug.LogError("Falta el prefab del proyectil en los moves del Spirithar!", this);
            return;
        }

        //Debug.Log($"Instantiating projectile with AttackLaunch rotation: {AttackLaunch.rotation.eulerAngles}");
       // targetPoint = transform.position + transform.transform.forward * 100f;
        Vector3 launchDirection = (_spiritharAi.PlayerAimTarget.position - AttackLaunch.position).normalized;

        GameObject projectile = Instantiate(
            this.moves[0].SoulProjectilePrefab,
            AttackLaunch.position,
             Quaternion.LookRotation(launchDirection)
        );

        SoulProjectile soulProjectile = projectile.GetComponent<SoulProjectile>();

        if (soulProjectile != null) {
            // Llamada corregida: sin pasar 'transform'
            soulProjectile.Initialize(this.moves[0], _shootForwardForce, _shootUpForce, launchDirection);
        } else {
            Debug.LogError("El prefab del proyectil no tiene el script SoulProjectile!", projectile);
        }
    }
        /*public void ShootProjectile(Transform objective) {
            print("SHOOOOT PROJECTILEEEE");
            //Vector3 shootingPosition = transform.position;
        //GameObject projectile = Instantiate(this.moves[0].SoulProjectilePrefab, AttackLaunch.position, Quaternion.identity);
        GameObject projectile = Instantiate(
            this.moves[0].SoulProjectilePrefab,
            AttackLaunch.position,
            transform.rotation 
        );
        SoulProjectile soulProjectile = projectile.GetComponent<SoulProjectile>();
        if (soulProjectile != null) {
            // Pasa solo los datos necesarios, no el transform del shooter
            soulProjectile.Initialize(this.moves[0], transform, _shootForwardForce, _shootUpForce);
        } else {
            Debug.LogError("El prefab del proyectil no tiene el script SoulProjectile!", projectile);
        }
        //soulProjectile.Initialize(this.moves[0], transform, _shootForwardForce, _shootUpForce);
        *//*projectile.GetComponent<Rigidbody>().AddForce(transform.forward * _shootForwardForce, ForceMode.Impulse);
        projectile.GetComponent<Rigidbody>().AddForce(transform.up * _shootUpForce, ForceMode.Impulse);*//*
        
    }*/

    public override void PerformMove(SpiritharMove spiritharMove, Spirithar target, bool isSpiritharFromTeam = false) {
        Debug.LogWarning("PerformMove not used in Soul Combat mode");
    }

    public override bool IsWeak(SpiritharMove spiritharMove) {
        return spiritharMove.moveElementType == ElementType.Water;
    }

}