// Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour {
    private SpiritharMove _move;
    private float _playerAttack;

    public void Initialize(SpiritharMove move, float playerAttack, Vector3 direction) {
        _move = move;
        _playerAttack = playerAttack;
        print("Inicializooo proyectiiil");

        // Aplicar fuerza en la dirección calculada
        GetComponent<Rigidbody>().AddForce(direction * _move.projectileSpeed, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other) {
        bool isDead;
        print($"Entro en colision {this.gameObject.name}");
        if (other.CompareTag("SoulSpirithar")) {
            print($"Entro en colision contra SOUL");
            Spirithar enemy = other.gameObject.GetComponent<Spirithar>();
            float damage = _move.power + (_playerAttack * 0.7f);
            if (enemy.IsWeak(_move))
                damage *= 1.2f;
            isDead = enemy.TakeDamage(damage);
            if (isDead) {
                enemy.Die();
            } 
            //Instantiate(_move.impactVFX, transform.position, Quaternion.identity);

            Destroy(gameObject);

        }
    }

}