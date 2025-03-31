using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class SoulProjectile : MonoBehaviour
{
    private SpiritharMove _move;
    private bool _hasCollided = false;
    public void Initialize(SpiritharMove move, Transform transform, int shootForwardForce, int shootUpForce) {
        _move = move;

        GetComponent<Rigidbody>().AddForce(transform.forward * shootForwardForce, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(transform.up * shootUpForce, ForceMode.Impulse);
        _hasCollided = false;
        Destroy(gameObject, 3f);
    }
    private void OnTriggerEnter(Collider other) {
        if (_hasCollided) return;

        if (other.CompareTag("Player")) {
            _hasCollided = true;
            PlayerSoulCombatAndStats playerStats = other.GetComponent<PlayerSoulCombatAndStats>();
            print($"{_move.power}");
            playerStats.TakeDamage(_move.power);
            Debug.Log("Hit Player");
            Destroy(gameObject);



        }
    }
}
