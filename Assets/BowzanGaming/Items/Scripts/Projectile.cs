// Projectile.cs
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour {
    #region Soul Combat Variables
    private SpiritharMove _move;
    private float _playerAttack;
    #endregion

    #region Turn Based Variables
    [SerializeField] private float _duration = 1f; // Duración en segundos
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _elapsedTime;
    private Vector3 _velocity = Vector3.zero;
    #endregion

    #region Soul Combat projectil movement
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
    #endregion

    #region Turn Based projectil movement
    public void TurnBasedInitialize(Vector3 target) {
        _startPosition = transform.position;
        _targetPosition = target;
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget() {
        _elapsedTime = 0f;

        while (_elapsedTime < _duration) {
            // Calcular la interpolación
            float t = _elapsedTime / _duration;
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);

            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurar posición final exacta
        transform.position = _targetPosition;
        Destroy(gameObject);
    }
    #endregion

}