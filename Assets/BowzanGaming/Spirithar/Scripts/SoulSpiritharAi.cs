using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoulSpiritharAi : MonoBehaviour
{
    private NavMeshAgent _agent;
    private SoulSpirithar _soulSpirithar;
    [SerializeField] private bool _lockVerticalRotation = true;
    [SerializeField] private float _rotationSpeed = 5f;

    public Transform Player;

    public LayerMask WhatIsGround, WhatIsPlayer;

    // Patroling
    public Vector3 WalkPoint;
    bool _walkPointSet;
    public float WalkPointRange;

    //Attacking
    public float TimeBetweenAttacks;
    bool _alreadyAttacked;

    // States
    public float SightRange, AttackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    private void Awake() {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _soulSpirithar = GetComponent<SoulSpirithar>();
    }

    private void Update() {
        // Check for sight and attack range 
        PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, WhatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, WhatIsPlayer);

        //if (!PlayerInSightRange && !PlayerInAttackRange) Patroling();
        if (_soulSpirithar.CurrentStateMode == EnemyState.Combat) {
            if (PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();
            if (PlayerInSightRange && PlayerInAttackRange) AttackPlayer();
        } else
            Patroling();
        
    }

    private void Patroling() {

        if (!_walkPointSet) SearchWalkPoint();

        if(_walkPointSet) _agent.SetDestination(WalkPoint);

        Vector3 distanceToWalkPoint = transform.position - WalkPoint;

        //WalkPoint reached
        if(distanceToWalkPoint.magnitude < 1f) {
            _walkPointSet = false;
        }

    }

    private void SearchWalkPoint() {
        //Calculate random point in range
        float randomZ = Random.Range(-WalkPointRange, WalkPointRange);
        float randomX = Random.Range(-WalkPointRange, WalkPointRange);

        WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(WalkPoint, -transform.up, 2f, WhatIsGround)) {
            _walkPointSet = true;
        }
    }

    private void ChasePlayer() {
        _agent.SetDestination(Player.position);
    }

    private void AttackPlayer() {
        // Make sure enemy doesnt move
        _agent.SetDestination(transform.position);

        //transform.LookAt(Player);
        Vector3 direction = Player.position - transform.position;

        // Opcional: Bloquear rotación en el eje X/Z
        if (_lockVerticalRotation) {
            direction.y = 0f;
        }

        // Calcular la rotación deseada
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Aplicar rotación suavizada
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );

        if (!_alreadyAttacked) {

            // Attackcode here
            print("ATTACKKKK SOUL AI");
            _soulSpirithar.ShootProjectile(Player);

            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        _alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, SightRange);

    }
}
