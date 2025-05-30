using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI; // Aseg�rate de tener esto

public class SoulSpiritharAi : MonoBehaviour {
    private NavMeshAgent _agent;
    private SoulSpirithar _soulSpirithar;
    // Ya no necesitas _lockVerticalRotation si siempre quieres rotaci�n completa
    // [SerializeField] private bool _lockVerticalRotation = true;
    [SerializeField] private float _rotationSpeed = 5f;
    [Tooltip("El �ngulo m�nimo (en grados) que debe haber antes de que el AI intente rotar.")]
    [SerializeField] private float _minRotationAngleThreshold = 1.0f; // Umbral para evitar jitter
    [SerializeField] private float _aimHeightOffset = 1.5f;
    [Header("Targeting")]
    [Tooltip("El punto espec�fico en el jugador al que apuntar. Si es null, usar� Player.position + offset.")]
    public Transform PlayerAimTarget;
    [SerializeField] private float _fallbackAimHeightOffset = 1.5f; // Offset a usar si PlayerAimTarget no est� asignado

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
                Debug.LogWarning($"No se encontr� 'AimTargetPoint' como hijo de {Player.name}. Usando offset de altura como fallback.");
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
            } else if (PlayerInSightRange) { // Solo perseguir si est� a la vista pero no en rango de ataque
                ChasePlayer();
            } else { // Si est� en combate pero perdi� de vista al jugador
                // Podr�as hacerlo patrullar o buscar al jugador aqu�
                Patroling(); // O quiz�s ChasePlayer() hacia la �ltima posici�n conocida?
            }
        } else { // Si no est� en modo Combate
            Patroling();
        }

        HandleRotation();
        if (_agent.updateRotation) Debug.Log("Update rotaatiooon");
    }

    void HandleRotation() {
        Quaternion targetRotation;

        // Si el jugador est� a la vista, mirar hacia �l (con ajuste vertical)
        if (PlayerInSightRange && Player != null && _soulSpirithar.CurrentStateMode == EnemyState.Combat) {
            Vector3 targetPosition;
            if (PlayerAimTarget != null) {
                targetPosition = PlayerAimTarget.position;

            } else {
                targetPosition = Player.position + (Vector3.up * _aimHeightOffset);

            }

            Vector3 directionToPlayer = targetPosition - transform.position;
            //Debug.Log($"{directionToPlayer} direction of Spirithar to Player and magnitud of dir {directionToPlayer.sqrMagnitude}");
            // Solo calcular rotaci�n si la direcci�n no es cero
            if (directionToPlayer.sqrMagnitude > 0.001f) {
                // --- PRUEBA CON LOOKAT ---
                // Comenta el bloque Slerp de abajo y descomenta esta l�nea:
                //Debug.Log($"[{Time.frameCount}] Forzando LookAt hacia: {targetPosition}");
                transform.LookAt(targetPosition);
                // --- FIN PRUEBA LOOKAT ---

                /*targetRotation = Quaternion.LookRotation(directionToPlayer);

                // Aplicar Slerp solo si el �ngulo es suficiente (evita jitter)
                float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
                if (angleDifference > _minRotationAngleThreshold) {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                    
                    if (_agent.updateRotation) Debug.Log("Update Rotation Agent");
                }*/
                // Si el �ngulo es peque�o, no rotamos, manteniendo la estabilidad.

            } // Si la direcci�n es cero, no hacemos nada, mantenemos rotaci�n actual.

        }
        // Si el jugador NO est� a la vista, mirar en la direcci�n del movimiento del agente
        else if (_agent != null && _agent.velocity.sqrMagnitude > 0.1f) // Comprobar si se est� moviendo
        {
            Vector3 directionOfMovement = _agent.velocity.normalized;
            targetRotation = Quaternion.LookRotation(directionOfMovement);
            // Aqu� podr�amos usar un Slerp m�s r�pido o directo si queremos que se alinee r�pido con el movimiento
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        // Si no est� a la vista y no se mueve, simplemente mantiene su rotaci�n actual.
    }


    private void Patroling() {
        // --- Reactivar Agente ---
        if (_agent != null && _agent.isStopped) {
            // Asegurarse de que puede moverse
            _agent.isStopped = false;
        }

        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet && _agent != null) _agent.SetDestination(WalkPoint);

        // Solo comprobar si est� cerca si el agente est� activo y tiene un path
        if (_agent != null && !_agent.pathPending && _agent.remainingDistance < 1.0f) {
            _walkPointSet = false;
        }
    }

    private void SearchWalkPoint() {
        //Calculate random point in range
        float randomZ = Random.Range(-WalkPointRange, WalkPointRange);
        float randomX = Random.Range(-WalkPointRange, WalkPointRange);

        WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Comprobar si el punto es v�lido en el NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(WalkPoint, out hit, 5.0f, NavMesh.AllAreas)) {
            // Usar el punto encontrado en el NavMesh m�s cercano
            WalkPoint = hit.position;
            _walkPointSet = true;
           /* // Comprobar si hay suelo debajo (opcional si SamplePosition es suficiente)
            if (Physics.Raycast(WalkPoint + Vector3.up * 0.5f, Vector3.down, 1f, WhatIsGround)) {
                
            }*/
        } else {
            // No se encontr� un punto v�lido cerca, intentar de nuevo en el pr�ximo frame
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
                                    //    S�male el vector 'arriba' (Vector3.up) escalado por tu offset
      /*  Vector3 targetAimPosition;
        // --- DEBUG PlayerAimTarget ---
        if (PlayerAimTarget == null) {
            Debug.LogError($"[{Time.frameCount}] PlayerAimTarget is NULL. Attempting fallback.");
            // Intentar usar el fallback si PlayerAimTarget es null
            targetAimPosition = Player.position + (Vector3.up * _fallbackAimHeightOffset);
        } else {
            targetAimPosition = PlayerAimTarget.position;
            // Log opcional para ver la posici�n del target asignado
            // Debug.Log($"[{Time.frameCount}] Using PlayerAimTarget at {targetAimPosition}");
        }
        // --- FIN DEBUG ---
        //Vector3 targetAimPosition = Player.position + (Vector3.up * _aimHeightOffset);

        // 3. Calcula la direcci�n desde el Spirithar hacia ese nuevo punto objetivo
        //Vector3 direction = targetAimPosition - transform.position;
        // Log para ver posiciones y direcci�n calculada
        Debug.Log($"[{Time.frameCount}] Spirithar Y: {transform.position.y:F2}, Target Y: {targetAimPosition.y:F2}");
        Vector3 direction = targetAimPosition - transform.position;
        Debug.Log($"[{Time.frameCount}] Calculated Direction: {direction}"); // �Mira si direction.y es distinto de cero!

        //Vector3 direction = Player.position - transform.position;
        // No necesitamos normalizar para LookRotation, pero s� asegurarnos de que no sea zero
        if (direction.sqrMagnitude > 0.001f) // Evitar LookRotation con vector cero
        {
            // Calcular la rotaci�n deseada (incluye eje Y)
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // --- A�adir umbral para evitar jitter ---
            // Calcular el �ngulo entre la direcci�n actual y la deseada
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // Solo rotar si el �ngulo es mayor que el umbral
            if (angleDifference > _minRotationAngleThreshold) {
                // Aplicar rotaci�n suavizada
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                );
            }
            // Si el �ngulo es muy peque�o, no hacemos nada, manteniendo la rotaci�n actual estable.
            //-----------------------------------------
        }*/


        // L�gica de ataque
        if (!_alreadyAttacked) {
            // Comprobar si est� razonablemente encarado antes de disparar (opcional pero recomendado)
            // Podr�as usar un �ngulo mayor aqu� si quieres que est� bien alineado
            // float alignmentAngle = Vector3.Angle(transform.forward, direction.normalized);
            // if (alignmentAngle < 10.0f) // Ejemplo: disparar si est� a menos de 10 grados

            // Attackcode here
            print("ATTACKKKK SOUL AI");
            _soulSpirithar.ShootProjectile(Player); // Asumiendo que ShootProjectile maneja la direcci�n

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

    // Es buena pr�ctica asegurarse de que el agente se detiene si el objeto se desactiva
    private void OnDisable() {
        if (_agent != null && _agent.isOnNavMesh) {
            /*_agent.isStopped = true;
            _agent.updateRotation = true; // Restaurar por si acaso*/
        }
    }

    // Y si se reactiva, que pueda moverse si es necesario (aunque Update lo manejar�)
    private void OnEnable() {
        if (_agent != null) {
            // El estado se determinar� en el pr�ximo Update
        }
    }
}