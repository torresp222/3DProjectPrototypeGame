using BowzanGaming.FinalCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerSoulCombatHandler : MonoBehaviour
{
    public static event Action OnSpellInstance;

    [Header("References for launch projectile")]
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private Camera _playerCamera;

    private PlayerSoulCombatAndStats _playerStats;
    private AbsorptionManager _absorptionManager;
    private bool _projectileInitialize;

    private void Awake() {
        _playerStats = GetComponent<PlayerSoulCombatAndStats>();
        _absorptionManager = GetComponent<AbsorptionManager>();
    }
    private void OnEnable() {
        OnSpellInstance += PerformSpell;
    }

    private void OnDisable() {
        OnSpellInstance -= PerformSpell;
    }

    public void SetProjectileInitializeTrue() { OnSpellInstance?.Invoke(); }

    public void PerformSpell() {
        var move = _absorptionManager.GetCurrentMove(0);
        if (move == null) {
            return;
        } 

        LaunchProjectile(move);
    }

    public void PerformDefenseBoost() {
        var move = _absorptionManager.GetCurrentMove(1);
        if (move == null) return;

        _playerStats.ApplyDefenseBoost(1 + (move.power / 100f));
        //PlayVFX(move.vfx);
    }

    public void PerformAttackBoost() {
        var move = _absorptionManager.GetCurrentMove(2);
        if (move == null) return;

        _playerStats.ApplyAttackBoost(1 + (move.power / 100f));
       // PlayVFX(move.vfx);
    }

    private void LaunchProjectile(SpiritharMove move) {
        // 1. Calcular punto de mira
        Vector3 targetPoint;
        Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Centro de la pantalla
        if (Physics.Raycast(ray, out RaycastHit hit, 100f)) {
            targetPoint = hit.point;
        } else {
            // Si no hay impacto, usar punto lejano en la dirección de la cámara
            targetPoint = _playerCamera.transform.position + _playerCamera.transform.forward * 100f;
        }

        // 2. Calcular dirección desde el punto de lanzamiento al target
        Vector3 launchDirection = (targetPoint - _projectileSpawnPoint.position).normalized;

        // 3. Instanciar y lanzar
        GameObject projectile = Instantiate(move.projectilePrefab, _projectileSpawnPoint.position, Quaternion.LookRotation(launchDirection));
        Projectile projComponent = projectile.GetComponent<Projectile>();
        projComponent.Initialize(move, _playerStats.currentAttack, launchDirection);

        Debug.Log("Proyectil lanzado hacia: " + targetPoint);

        /*GameObject projectile = Instantiate(move.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Projectile projComponent = projectile.GetComponent<Projectile>();
        print("Inicializar PORYECTILLLL");
        projComponent.Initialize(move, _playerStats.currentAttack, _playerCamera.transform);*/


    }

    private void OnDrawGizmos() {
        if (_playerCamera == null || _projectileSpawnPoint == null) return;

        // 1. Calcular el rayo desde el centro de la cámara
        Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // 2. Configurar colores
        Gizmos.color = Color.red; // Rayo original de la cámara
        Color projectileColor = Color.green; // Trayectoria corregida

        // 3. Dibujar rayo de la cámara
        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100f);

        // 4. Calcular dirección corregida
        if (Physics.Raycast(ray, out RaycastHit hit, 100f)) {
            // Dirección desde el punto de lanzamiento al impacto
            Vector3 correctedDirection = (hit.point - _projectileSpawnPoint.position).normalized;

            // Dibujar trayectoria corregida
            Gizmos.color = projectileColor;
            Gizmos.DrawLine(_projectileSpawnPoint.position, hit.point);

            // Dibujar esfera en el punto de impacto
            Gizmos.DrawWireSphere(hit.point, 0.2f);
        } else {
            // Dirección si no hay impacto
            Vector3 farPoint = ray.origin + ray.direction * 100f;
            Vector3 correctedDirection = (farPoint - _projectileSpawnPoint.position).normalized;

            Gizmos.color = projectileColor;
            Gizmos.DrawLine(_projectileSpawnPoint.position, farPoint);
        }

        // 5. Etiquetas informativas (solo en Editor)
        // 5. Etiquetas informativas (solo en Editor)
        #if UNITY_EDITOR
                Handles.Label(_projectileSpawnPoint.position, "Spawn Point", new GUIStyle() { normal = new GUIStyleState() { textColor = Color.white } });
                Handles.Label(ray.origin + ray.direction * 5f, "Rayo de la Mira", new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red } });
        #endif
    }

    private void PlayVFX(GameObject vfx) {
        Instantiate(vfx, transform.position + Vector3.up, Quaternion.identity);
    }
}
