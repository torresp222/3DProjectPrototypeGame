using BowzanGaming.FinalCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;
public class CombatManager : MonoBehaviour {

    public static CombatManager Instance;

    [Header("Configuración de Combate")]
    [Tooltip("Distancia entre el Spirithar salvaje y el del jugador")]
    public float CombatDistance = 5f;
    [Tooltip("Offset para posicionar al jugador detrás de su Spirithar (en espacio local)")]
    public Vector3 PlayerOffset = new Vector3(0, 0, -2f);

    [Header("Cámaras")]
    public GameObject CombatCamera;   // Cámara exclusiva para el combate
    public GameObject PlayerCamera;   // Cámara de exploración (jugador)

    [Header("Referencias")]
    public GameObject Player;         // GameObject del jugador
    public PlayerTeam PlayerTeam;     // Script que gestiona el equipo del jugador
    public BowzanGaming.FinalCharacterController.PlayerInputManager RefPlayerInputManager; // Player Input Manager que habilita el Input system "Player Controls"
    //public PlayerLocomotionInput Pli;
    public GameObject CombatCaptureUI; // GameObject donde se encuentra el Canvas del combate entre spirithars

    // Referencias internas que se asignarán al iniciar el combate.
    private Spirithar _enemySpirithar; // Spirithar salvaje capturado
    private Spirithar _playerSpirithar; // Spirithar activo del equipo (instanciado en combate)
    private GameObject _firstSpiritharTeam;// GameObject "" ""
    private CombatCaptureHUD _combatCaptureHUD; // Reference to script that controls ui of combate capture.

    private void Awake() {
        // Patrón singleton para acceso global.
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (CombatCaptureUI != null) {
            _combatCaptureHUD = CombatCaptureUI.GetComponent<CombatCaptureHUD>();
            CombatCaptureUI.SetActive(false);
        }
            
    }
    private void OnEnable() {
        CaptureBall.OnSpiritharCaptured += InitiateCombat;
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharCaptured -= InitiateCombat;
    }

    // Este método se llamará cuando se capture un Spirithar.
    private void InitiateCombat(Spirithar capturedSpirithar) {
        Debug.Log("Iniciando combate con: " + capturedSpirithar.spiritharName);
        StartCombat(capturedSpirithar);
    }

    /// <summary>
    /// Inicia el combate, instanciando el Spirithar activo del equipo y posicionando a todos los participantes.
    /// </summary>
    /// <param name="capturedEnemySpirithar">El Spirithar salvaje capturado.</param>
    public void StartCombat(Spirithar capturedEnemySpirithar) {
        // Asignar el Spirithar salvaje y conservar su posición actual.
        _enemySpirithar = capturedEnemySpirithar;
        Vector3 enemyPosition = _enemySpirithar.transform.position;

        // Deshabilitar controles y la cámara del jugador.
        DisablePlayerControl();
        if (PlayerCamera != null)
            PlayerCamera.SetActive(false);

        // Instanciar el Spirithar activo del equipo a partir del prefab guardado en PlayerTeam.
        _firstSpiritharTeam = PlayerTeam.GetActiveSpiritharPrefab();
        if (_firstSpiritharTeam == null) {
            Debug.LogError("No se encontró un prefab de Spirithar en el equipo del jugador.");
            return;
        }
        //GameObject playerSpiritharGO
        _firstSpiritharTeam = Instantiate(_firstSpiritharTeam, Vector3.zero, Quaternion.identity);
        _playerSpirithar = _firstSpiritharTeam.GetComponent<Spirithar>();
        if (_playerSpirithar == null) {
            Debug.LogError("El prefab instanciado no tiene el componente Spirithar.");
            return;
        }

        // Posicionar al _enemySpirithar: se mantiene en su posición actual.
        // Posicionar al playerSpirithar: se coloca a una distancia CombatDistance del enemigo, en dirección opuesta al frente del enemigo.
        Vector3 direction = -_enemySpirithar.transform.forward;
        Vector3 playerSpiritharPos = enemyPosition + direction * CombatDistance;
        _playerSpirithar.transform.position = playerSpiritharPos;
        // Hacer que el Spirithar del jugador mire al enemigo y viceversa
        _enemySpirithar.transform.LookAt(playerSpiritharPos);
        _playerSpirithar.transform.LookAt(enemyPosition);

        // Posicionar al jugador detrás de su Spirithar utilizando el offset (convertido de local a global).
        Vector3 globalPlayerOffset = _playerSpirithar.transform.TransformDirection(PlayerOffset);
        Player.transform.position = _playerSpirithar.transform.position + globalPlayerOffset;
        Player.transform.LookAt(enemyPosition);

        // Display Name of Spirithars
        _combatCaptureHUD.SetUpCaptureCombat(_playerSpirithar, _enemySpirithar);

        // Activar la cámara de combate.
        if (CombatCamera != null)
            CombatCamera.SetActive(true);
        if(CombatCaptureUI != null)
            CombatCaptureUI.SetActive(true);
        

        Debug.Log("Combate iniciado: " + _playerSpirithar.spiritharName + " vs " + _enemySpirithar.spiritharName);
    }

    /// <summary>
    /// Deshabilita el movimiento y la interacción del jugador.
    /// Se asume que el jugador tiene componentes PlayerController y PlayerInput.
    /// </summary>
    private void DisablePlayerControl() {
        //Desactivar inputs del jugador
        if (RefPlayerInputManager != null) {
            RefPlayerInputManager.enabled = false;
        }
        // Desactivar el script de movimiento.
        PlayerController pc = Player.GetComponent<PlayerController>();
        if (pc != null)
            pc.enabled = false;

        // Desactivar PlayerInput para evitar capturar más acciones.
        PlayerLocomotionInput pli = Player.GetComponent<PlayerLocomotionInput>();
        if (pli != null)
            pli.enabled = false;

        PlayerActionsInput pai = Player.GetComponent<PlayerActionsInput>();
        if (pai != null)
            pai.enabled = false;
    }

    /// <summary>
    /// (Opcional) Reactiva el control del jugador tras finalizar el combate.
    /// </summary>
    public void EnablePlayerControl() {
        if (RefPlayerInputManager != null) {
            RefPlayerInputManager.enabled = true;
        }

        PlayerController pc = Player.GetComponent<PlayerController>();
        if (pc != null)
            pc.enabled = true;

        PlayerLocomotionInput pli = Player.GetComponent<PlayerLocomotionInput>();
        if (pli != null)
            pli.enabled = true;

        PlayerActionsInput pai = Player.GetComponent<PlayerActionsInput>();
        if (pai != null)
            pai.enabled = true;

        // Desactivar la cámara de combate.
        if (CombatCamera != null)
            CombatCamera.SetActive(false);
        if (CombatCaptureUI != null)
            CombatCaptureUI.SetActive(false);

        if (PlayerCamera != null)
            PlayerCamera.SetActive(true);

        Destroy(_firstSpiritharTeam);
    }
}
