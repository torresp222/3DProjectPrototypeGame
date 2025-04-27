using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI; // Asegúrate de tener esto

public class SoulSpiritharAi : MonoBehaviour {
    private NavMeshAgent _agent;
    private SoulSpirithar _soulSpirithar;
    // Ya no necesitas _lockVerticalRotation si siempre quieres rotación completa
    // [SerializeField] private bool _lockVerticalRotation = true;
    [SerializeField] private float _rotationSpeed = 5f;
    [Tooltip("El ángulo mínimo (en grados) que debe haber antes de que el AI intente rotar.")]
    [SerializeField] private float _minRotationAngleThreshold = 1.0f; // Umbral para evitar jitter
    [SerializeField] private float _aimHeightOffset = 1.5f;
    [Header("Targeting")]
    [Tooltip("El punto específico en el jugador al que apuntar. Si es null, usará Player.position + offset.")]
    public Transform PlayerAimTarget;
    [SerializeField] private float _fallbackAimHeightOffset = 1.5f; // Offset a usar si PlayerAimTarget no está asignado

    public Transform Player;

    public LayerMask WhatIsGround, WhatIsPlayer;

    // Patroling
    public Vector3 WalkPoint;
    public bool _walkPointSet;
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

        // Es buena idea asegurarse de que el agente puede rotar al inicio
        if (_agent != null) {
            _agent.updateRotation = false;
        }

        if (Player != null && PlayerAimTarget == null) {
            // Intenta encontrar un hijo llamado "AimTargetPoint"
            Transform foundTarget = Player.Find("AimTargetPoint");
            if (foundTarget != null) {
                PlayerAimTarget = foundTarget;
            } else {
                Debug.LogWarning($"No se encontró 'AimTargetPoint' como hijo de {Player.name}. Usando offset de altura como fallback.");
            }
        }
    }

    private void Update() {
        // Check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, WhatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, WhatIsPlayer);

        if (_soulSpirithar.CurrentStateMode == EnemyState.Combat) {
            if (PlayerInSightRange && PlayerInAttackRange) {
                AttackPlayer();
            } else if (PlayerInSightRange) { // Solo perseguir si está a la vista pero no en rango de ataque
                ChasePlayer();
            } else { // Si está en combate pero perdió de vista al jugador
                // Podrías hacerlo patrullar o buscar al jugador aquí
                Patroling(); // O quizás ChasePlayer() hacia la última posición conocida?
            }
        } else { // Si no está en modo Combate
            Patroling();
        }

        HandleRotation();
        if (_agent.updateRotation) Debug.Log("Update rotaatiooon");
    }

    void HandleRotation() {
        Quaternion targetRotation;

        // Si el jugador está a la vista, mirar hacia él (con ajuste vertical)
        if (PlayerInSightRange && Player != null && _soulSpirithar.CurrentStateMode == EnemyState.Combat) {
            Vector3 targetPosition;
            if (PlayerAimTarget != null) {
                targetPosition = PlayerAimTarget.position;

            } else {
                targetPosition = Player.position + (Vector3.up * _aimHeightOffset);

            }

            Vector3 directionToPlayer = targetPosition - transform.position;
            //Debug.Log($"{directionToPlayer} direction of Spirithar to Player and magnitud of dir {directionToPlayer.sqrMagnitude}");
            // Solo calcular rotación si la dirección no es cero
            if (directionToPlayer.sqrMagnitude > 0.001f) {
                // --- PRUEBA CON LOOKAT ---
                // Comenta el bloque Slerp de abajo y descomenta esta línea:
                //Debug.Log($"[{Time.frameCount}] Forzando LookAt hacia: {targetPosition}");
                transform.LookAt(targetPosition);
                // --- FIN PRUEBA LOOKAT ---

                /*targetRotation = Quaternion.LookRotation(directionToPlayer);

                // Aplicar Slerp solo si el ángulo es suficiente (evita jitter)
                float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
                if (angleDifference > _minRotationAngleThreshold) {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                    
                    if (_agent.updateRotation) Debug.Log("Update Rotation Agent");
                }*/
                // Si el ángulo es pequeño, no rotamos, manteniendo la estabilidad.

            } // Si la dirección es cero, no hacemos nada, mantenemos rotación actual.

        }
        // Si el jugador NO está a la vista, mirar en la dirección del movimiento del agente
        else if (_agent != null && _agent.velocity.sqrMagnitude > 0.1f) // Comprobar si se está moviendo
        {
            Vector3 directionOfMovement = _agent.velocity.normalized;
            targetRotation = Quaternion.LookRotation(directionOfMovement);
            // Aquí podríamos usar un Slerp más rápido o directo si queremos que se alinee rápido con el movimiento
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        // Si no está a la vista y no se mueve, simplemente mantiene su rotación actual.
    }


    private void Patroling() {
        // --- Reactivar Agente ---
        if (_agent != null && _agent.isStopped) {
            // Asegurarse de que puede moverse
            _agent.isStopped = false;
        }

        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet && _agent != null) _agent.SetDestination(WalkPoint);

        // Solo comprobar si está cerca si el agente está activo y tiene un path
        if (_agent != null && !_agent.pathPending && _agent.remainingDistance < 1.0f) {
            _walkPointSet = false;
        }
    }

    private void SearchWalkPoint() {
        //Calculate random point in range
        float randomZ = Random.Range(-WalkPointRange, WalkPointRange);
        float randomX = Random.Range(-WalkPointRange, WalkPointRange);

        WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Comprobar si el punto es válido en el NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(WalkPoint, out hit, 5.0f, NavMesh.AllAreas)) {
            // Usar el punto encontrado en el NavMesh más cercano
            WalkPoint = hit.position;
            _walkPointSet = true;
           /* // Comprobar si hay suelo debajo (opcional si SamplePosition es suficiente)
            if (Physics.Raycast(WalkPoint + Vector3.up * 0.5f, Vector3.down, 1f, WhatIsGround)) {
                
            }*/
        } else {
            // No se encontró un punto válido cerca, intentar de nuevo en el próximo frame
            _walkPointSet = false;
        }
    }

    private void ChasePlayer() {
        // Asegurarse de que el agente PUEDE moverse
        if (_agent != null && _agent.isStopped) {
            _agent.isStopped = false;
        }

        if (Player != null && _agent != null) {
            _agent.SetDestination(Player.position);
        }
    }

    private void AttackPlayer() {
        // Detener el MOVIMIENTO del agente
        if (_agent != null && !_agent.isStopped) {
            _agent.isStopped = true;
            // Opcional: Limpiar la ruta actual para evitar que intente moverse al detenerse
            //_agent.ResetPath();
        }

        /*if (_agent != null && _agent.enabled) {
            // Es importante detenerlo ANTES de desactivarlo si estaba en movimiento
            if (!_agent.isStopped) _agent.isStopped = true;
            _agent.enabled = false;
        }*/


        if (Player == null) return; // Salir si no hay jugador
                                    //    Súmale el vector 'arriba' (Vector3.up) escalado por tu offset
      /*  Vector3 targetAimPosition;
        // --- DEBUG PlayerAimTarget ---
        if (PlayerAimTarget == null) {
            Debug.LogError($"[{Time.frameCount}] PlayerAimTarget is NULL. Attempting fallback.");
            // Intentar usar el fallback si PlayerAimTarget es null
            targetAimPosition = Player.position + (Vector3.up * _fallbackAimHeightOffset);
        } else {
            targetAimPosition = PlayerAimTarget.position;
            // Log opcional para ver la posición del target asignado
            // Debug.Log($"[{Time.frameCount}] Using PlayerAimTarget at {targetAimPosition}");
        }
        // --- FIN DEBUG ---
        //Vector3 targetAimPosition = Player.position + (Vector3.up * _aimHeightOffset);

        // 3. Calcula la dirección desde el Spirithar hacia ese nuevo punto objetivo
        //Vector3 direction = targetAimPosition - transform.position;
        // Log para ver posiciones y dirección calculada
        Debug.Log($"[{Time.frameCount}] Spirithar Y: {transform.position.y:F2}, Target Y: {targetAimPosition.y:F2}");
        Vector3 direction = targetAimPosition - transform.position;
        Debug.Log($"[{Time.frameCount}] Calculated Direction: {direction}"); // ¡Mira si direction.y es distinto de cero!

        //Vector3 direction = Player.position - transform.position;
        // No necesitamos normalizar para LookRotation, pero sí asegurarnos de que no sea zero
        if (direction.sqrMagnitude > 0.001f) // Evitar LookRotation con vector cero
        {
            // Calcular la rotación deseada (incluye eje Y)
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // --- Añadir umbral para evitar jitter ---
            // Calcular el ángulo entre la dirección actual y la deseada
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // Solo rotar si el ángulo es mayor que el umbral
            if (angleDifference > _minRotationAngleThreshold) {
                // Aplicar rotación suavizada
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                );
            }
            // Si el ángulo es muy pequeño, no hacemos nada, manteniendo la rotación actual estable.
            //-----------------------------------------
        }*/


        // Lógica de ataque
        if (!_alreadyAttacked) {
            // Comprobar si está razonablemente encarado antes de disparar (opcional pero recomendado)
            // Podrías usar un ángulo mayor aquí si quieres que esté bien alineado
            // float alignmentAngle = Vector3.Angle(transform.forward, direction.normalized);
            // if (alignmentAngle < 10.0f) // Ejemplo: disparar si está a menos de 10 grados

            // Attackcode here
            print("ATTACKKKK SOUL AI");
            _soulSpirithar.ShootProjectile(Player); // Asumiendo que ShootProjectile maneja la dirección

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

    // Es buena práctica asegurarse de que el agente se detiene si el objeto se desactiva
    private void OnDisable() {
        if (_agent != null && _agent.isOnNavMesh) {
            /*_agent.isStopped = true;
            _agent.updateRotation = true; // Restaurar por si acaso*/
        }
    }

    // Y si se reactiva, que pueda moverse si es necesario (aunque Update lo manejará)
    private void OnEnable() {
        if (_agent != null) {
            // El estado se determinará en el próximo Update
        }
    }
}