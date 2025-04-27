using UnityEngine;

public class SoulProjectile : MonoBehaviour {
    private SpiritharMove _move;
    private bool _hasCollided = false;

    // Firma corregida: sin parámetro 'Transform transform'

    /*public void Initialize(SpiritharMove move, float playerAttack, Vector3 direction) {
        _move = move;
        _playerAttack = playerAttack;
        print("Inicializooo proyectiiil");

        // Aplicar fuerza en la dirección calculada
        GetComponent<Rigidbody>().AddForce(direction * _move.projectileSpeed, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }*/
    public void Initialize(SpiritharMove move, int shootForwardForce, int shootUpForce, Vector3 direction) {
        _move = move;
        _hasCollided = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) {
            // Usa el transform DEL PROYECTIL (this.transform implícito o explícito)
            Debug.Log($"Applying force in direction: {transform.forward}"); // Log para confirmar
            rb.AddForce(direction * _move.projectileSpeed, ForceMode.Impulse);

            // Decide si necesitas la fuerza 'up'
            // rb.AddForce(transform.up * shootUpForce, ForceMode.Impulse);
        } else {
            Debug.LogError("El proyectil no tiene Rigidbody!", this);
        }

        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other) {
        if (_hasCollided) return;

        if (other.CompareTag("Player")) {
            _hasCollided = true;
            PlayerSoulCombatAndStats playerStats = other.GetComponent<PlayerSoulCombatAndStats>();
            if (playerStats != null && _move != null) { // Añadir comprobación de null para _move
                print($"Applying damage: {_move.power}");
                playerStats.TakeDamage(_move.power);
            } else {
                Debug.LogWarning("PlayerStats o Move es null en el proyectil.");
            }
            Debug.Log("Hit Player");
            Destroy(gameObject);
        }
        // Podrías añadir lógica para colisiones con otros objetos si es necesario
        // else if (!other.CompareTag("Enemy")) { // Ejemplo: destruir si choca con algo que no sea el jugador o enemigo
        //     _hasCollided = true;
        //     Destroy(gameObject);
        // }
    }
}