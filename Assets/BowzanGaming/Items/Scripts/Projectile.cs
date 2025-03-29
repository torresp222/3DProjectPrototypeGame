// Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour {
    private SpiritharMove _move;
    private float _playerAttack;

    public void Initialize(SpiritharMove move, float playerAttack, Vector3 direction) {
        _move = move;
        _playerAttack = playerAttack;

        // Aplicar fuerza en la dirección calculada
        GetComponent<Rigidbody>().AddForce(direction * _move.projectileSpeed, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }

    /* public void Initialize(SpiritharMove move, float playerAttack, Transform transform) {
         _move = move;
         _playerAttack = playerAttack;
         //GetComponent<Rigidbody>().velocity = transform.forward * _move.projectileSpeed;
         print(_move.projectileSpeed);
         GetComponent<Rigidbody>().AddForce(transform.forward * _move.projectileSpeed, ForceMode.Impulse);
         Destroy(gameObject, 5f);
     }*/


    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("SoulSpirithar")) {
            Spirithar enemy = other.gameObject.GetComponent<Spirithar>();
            float damage = _move.power + (_playerAttack * 0.7f);
            if (enemy.IsWeak(_move))
                damage *= 1.2f;
            if (enemy.TakeDamage(damage)) {
                enemy.Die();
                print($"{enemy.spiritharName} DEAD");
            } else {
                print($"Damage {enemy.spiritharName} y vida actual {enemy.currentHealth}");
            }
            //Instantiate(_move.impactVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}