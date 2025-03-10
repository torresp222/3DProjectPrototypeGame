using BowzanGaming.FinalCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartingSpiritharBall : MonoBehaviour {
    [Header("Configuración del Spirithar Inicial")]
    // Prefab del Spirithar que se dará al recoger la bola.
    public GameObject SpiritharPrefab;

    private bool _playerInRange = false;
    private GameObject _player;  // Referencia al jugador que está en rango
    private PlayerActionsInput _playerActionsInput;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _playerInRange = true;
            _playerActionsInput = other.GetComponent<PlayerActionsInput>();
            _player = other.gameObject;
            //_playerActionsInput = other.GetComponent<PlayerActionsInput>();
            // Aquí podrías mostrar una UI (por ejemplo, "Pulsa E para recoger")
            Debug.Log("Jugador en rango para recoger la bola.");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            _playerInRange = false;
            _player = null;
            // Ocultar la UI de recogida, si la tienes
            Debug.Log("Jugador salió del rango de recogida.");
        }
    }

    private void Update() {
        // Usamos el Input System para detectar la pulsación de la tecla E.
        // Keyboard.current.eKey.wasPressedThisFrame devuelve true en el frame en que se pulsa E.
        if (_playerInRange && _playerActionsInput.GatherPressed /*Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame*/) {
            // Aquí se supone que el input E ya lanza la animación de gathering en el jugador.
            // Una vez pulsado E, se recoge la bola.
            Gather();
        }
    }

    private void Gather() {
        Debug.Log("Recogiendo la bola inicial y añadiendo el Spirithar al equipo del jugador.");

        // Suponemos que el jugador tiene un componente PlayerTeam que gestiona su equipo.
        PlayerTeam playerTeam = _player.GetComponent<PlayerTeam>();
        if (playerTeam != null) {
            Spirithar spiritharComponent = SpiritharPrefab.GetComponent<Spirithar>();
            if (spiritharComponent != null) {
                // Intentamos añadir el Spirithar al equipo.
                if (playerTeam.AddSpirithar(spiritharComponent)) {
                    // Si se añade correctamente, se destruye la bola para que no se recoja de nuevo.
                    Destroy(gameObject);
                } else {
                    Debug.Log("Equipo lleno");
                }
            } else {
                Debug.LogError("El prefab del Spirithar no tiene un componente Spirithar.");
            }
        }

        // Opcional: puedes reproducir efectos visuales o sonidos adicionales aquí.
    }
}
