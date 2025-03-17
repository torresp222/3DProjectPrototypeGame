using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerSoulCombatHandler : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Camera _playerCamera;

    private PlayerSoulCombatAndStats _playerStats;
    private AbsorptionManager _absorptionManager;

    private void Awake() {
        _playerStats = GetComponent<PlayerSoulCombatAndStats>();
        _absorptionManager = GetComponent<AbsorptionManager>();
    }

    public void PerformSpell() {
        var move = _absorptionManager.GetCurrentMove(0);
        if (move == null) return;

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
        GameObject projectile = Instantiate(move.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Projectile projComponent = projectile.GetComponent<Projectile>();
        projComponent.Initialize(move, _playerStats.currentAttack, _playerCamera.transform);
    }

    private void PlayVFX(GameObject vfx) {
        Instantiate(vfx, transform.position + Vector3.up, Quaternion.identity);
    }
}
