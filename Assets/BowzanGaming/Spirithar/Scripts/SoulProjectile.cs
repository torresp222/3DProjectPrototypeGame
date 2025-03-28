using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulProjectile : MonoBehaviour
{
    private SpiritharMove _move;
    public void Initialize(SpiritharMove move) {
        _move = move;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerSoulCombatAndStats playerStats = other.GetComponent<PlayerSoulCombatAndStats>();
            playerStats.TakeDamage(_move.power);
/*            Debug.Log("Hit Player");*/

            Destroy(gameObject);

        }
    }
}
