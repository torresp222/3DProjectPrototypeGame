using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public enum BattleCaptureState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class CombatManager : MonoBehaviour {

    public static CombatManager Instance;

    [Header("Configuraci�n de Combate")]
    [Tooltip("Distancia entre el Spirithar salvaje y el del jugador")]
    public float CombatDistance = 5f;
    [Tooltip("Offset para posicionar al jugador detr�s de su Spirithar (en espacio local)")]
    public Vector3 PlayerOffset = new Vector3(0, 0, -2f);

    [Header("C�maras")]
    public GameObject CombatCamera;   // C�mara exclusiva para el combate
    public GameObject PlayerCamera;   // C�mara de exploraci�n (jugador)

    [Header("Referencias")]
    public GameObject Player;         // GameObject del jugador
    public PlayerTeam PlayerTeam;     // Script que gestiona el equipo del jugador
    public BowzanGaming.FinalCharacterController.PlayerInputManager RefPlayerInputManager; // Player Input Manager que habilita el Input system "Player Controls"
    //public PlayerLocomotionInput Pli;
    public GameObject CombatCaptureUI; // GameObject donde se encuentra el Canvas del combate entre spirithars
    public GameObject CombatMenuGO;
    public GameObject AbilitiesMenu;
    public GameObject SpiritharMenu;
    [Header("Start Combat Menu")]
    public CombatCaptureHUD PlayerCombatCaptureHUD; // Reference to script that controls ui of combate capture.
    public CombatCaptureHUD EnemyCombatCaptureHUD; // Reference to script that controls ui of combate capture.
    [Header("Abilities Menu Buttons")]
    public AbilitiesCaptureHUD FirstButtonAbility;
    public AbilitiesCaptureHUD SecondButtonAbility;
    public AbilitiesCaptureHUD ThirdButtonAbility;
    [Header("Spirithar Menu Buttons")]
    public SpiritharCaptureHUD FirstSpiritharButton;
    public SpiritharCaptureHUD SecondSpiritharButton;
    public SpiritharCaptureHUD ThirdSpiritharButton;

    private List<SpiritharCaptureHUD> _spiritharCaptureHUDs;

    [Header("State of combate capture")]
    public BattleCaptureState State;

    // Referencias internas que se asignar�n al iniciar el combate.
    private Spirithar _enemySpirithar; // Spirithar salvaje capturado
    private Vector3 _enemyPosition;
    private Spirithar _playerSpirithar; // Spirithar activo del equipo (instanciado en combate)
    private GameObject _firstSpiritharTeam;// GameObject "" ""
    [SerializeField]private GameObject _currentSpiritharCombat;
    [SerializeField] private Vector3 _playerSpiritharPos;

    


    private void Awake() {
        // Patr�n singleton para acceso global.
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (CombatCaptureUI != null) {
            CombatCaptureUI.SetActive(false);
        }

        _spiritharCaptureHUDs = new List<SpiritharCaptureHUD> { FirstSpiritharButton, SecondSpiritharButton, ThirdSpiritharButton };
        Debug.Log(PlayerTeam.team.Count);

    }
    private void OnEnable() {
        CaptureBall.OnSpiritharCaptured += InitiateCombat;
        Spirithar.OnTakeDamage += UpdateSpiritharHPSlider;
        Spirithar.OnEnemySpiritharNotDead += ChangeState;
        Spirithar.OnSpiritharDead += EnablePlayerControl;
        SpiritharCaptureHUD.OnSpiritharClickToChange += PlaceSpiritharTeamInCombat;
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharCaptured -= InitiateCombat;
        Spirithar.OnTakeDamage -= UpdateSpiritharHPSlider;
        Spirithar.OnEnemySpiritharNotDead -= ChangeState;
        Spirithar.OnSpiritharDead -= EnablePlayerControl;
        SpiritharCaptureHUD.OnSpiritharClickToChange -= PlaceSpiritharTeamInCombat;
    }

    // Este m�todo se llamar� cuando se capture un Spirithar.
    private void InitiateCombat(Spirithar capturedSpirithar) {
        Debug.Log("Iniciando combate con: " + capturedSpirithar.spiritharName);
        State = BattleCaptureState.START;
        if (AbilitiesMenu != null)
            AbilitiesMenu.SetActive(false);
        if (SpiritharMenu != null)
            SpiritharMenu.SetActive(false);
        StartCombat(capturedSpirithar);
    }

    /// <summary>
    /// Inicia el combate, instanciando el Spirithar activo del equipo y posicionando a todos los participantes.
    /// </summary>
    /// <param name="capturedEnemySpirithar">El Spirithar salvaje capturado.</param>
    public void StartCombat(Spirithar capturedEnemySpirithar) {
        // Asignar el Spirithar salvaje y conservar su posici�n actual.
        _enemySpirithar = capturedEnemySpirithar;
        _enemyPosition = _enemySpirithar.transform.position;

        // Instanciar el Spirithar activo del equipo a partir del prefab guardado en PlayerTeam.
        _firstSpiritharTeam = PlayerTeam.GetActiveSpiritharPrefab();
        if (_firstSpiritharTeam == null) {
            Debug.LogError("No se encontr� un prefab de Spirithar en el equipo del jugador.");
            return;
        }

        // Place the spirithar of team in combat
        PlaceSpiritharTeamInCombat(_firstSpiritharTeam);
        
        // Set the menu of spirithars in team
        SetSpiritharMenu();

        // Deshabilitar controles y la c�mara del jugador.
        DisablePlayerControl();

        // Activar la c�mara de combate.
        if (CombatCamera != null)
            CombatCamera.SetActive(true);
        if(CombatCaptureUI != null)
            CombatCaptureUI.SetActive(true);
        if (PlayerCamera != null)
            PlayerCamera.SetActive(false);
        if (!CombatMenuGO.activeSelf) {
            Debug.Log("GO DE COMBATMENUGOOOOO NOOO ESTABA ACTIVOO Y AHORA SIII");
            CombatMenuGO.SetActive(true);
        }

        State = BattleCaptureState.PLAYERTURN;

       
    }

    public void SetSpiritharMenu() {
        // Recorrer todos los elementos del HUD
        for (int i = 0; i < _spiritharCaptureHUDs.Count; i++) {
            Spirithar teamMember = PlayerTeam.team[i];
            _spiritharCaptureHUDs[i].SetUpTextSpiritharNameButton(teamMember);
        }
    }

    /// <summary>
    /// Deshabilita el movimiento y la interacci�n del jugador.
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

        // Desactivar PlayerInput para evitar capturar m�s acciones.
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

        // Desactivar la c�mara de combate.
        if (CombatCamera != null)
            CombatCamera.SetActive(false);
        if (CombatCaptureUI != null)
            CombatCaptureUI.SetActive(false);

        if (PlayerCamera != null)
            PlayerCamera.SetActive(true);

        Destroy(_currentSpiritharCombat);
    }

    public void UpdateSpiritharHPSlider() {
        PlayerCombatCaptureHUD.SetHP(_playerSpirithar);
        EnemyCombatCaptureHUD.SetHP(_enemySpirithar);

    }

    public void ChangeState() {

        if (State == BattleCaptureState.PLAYERTURN) {
            State = BattleCaptureState.ENEMYTURN;
            _enemySpirithar.PerformingMove = false;
            StartCoroutine(EnemyTurn());
        } else 
            return;
            
        /*if (State == BattleCaptureState.ENEMYTURN) {
            State = BattleCaptureState.PLAYERTURN;
            Debug.Log("ENTROOO AL PLAYER TUUUURRRNNNN");
        }*/
    }

    public IEnumerator EnemyTurn() {
        Debug.Log("Turno del enemigo");
        SpiritharMove move = _enemySpirithar.moves[Random.Range(0, _enemySpirithar.moves.Length)];
        _enemySpirithar.PerformMove(move, _playerSpirithar);
        yield return new WaitForSeconds(2f);
        State = BattleCaptureState.PLAYERTURN;
        _playerSpirithar.PerformingMove = false;

    }

    // Logic for changing and placing spirithar in turn based combat
    private void PlaceSpiritharTeamInCombat(GameObject spiritharGO) {

        if (_currentSpiritharCombat == spiritharGO)
            return;

        if(_currentSpiritharCombat != null) {
            Destroy(_currentSpiritharCombat);
            Debug.Log("Destruyo el spirithar current!!!!");
        }
        Vector3 direction = -_enemySpirithar.transform.forward;
        Vector3 playerSpiritharPos = _enemyPosition + direction * CombatDistance;
        if (_playerSpiritharPos == Vector3.zero)
            _playerSpiritharPos = playerSpiritharPos;
        //GameObject playerSpiritharGO
        _currentSpiritharCombat = Instantiate(spiritharGO, _playerSpiritharPos, Quaternion.identity);
        Spirithar playerSpirithar = _currentSpiritharCombat.GetComponent<Spirithar>();

        if (playerSpirithar == null) {
            Debug.LogError("El prefab instanciado no tiene el componente Spirithar.");
            return;
        }
        
        // Posicionar al _enemySpirithar: se mantiene en su posici�n actual.
        // Posicionar al playerSpirithar: se coloca a una distancia CombatDistance del enemigo, en direcci�n opuesta al frente del enemigo.
        
        playerSpirithar.transform.position = _playerSpiritharPos;
        // Hacer que el Spirithar del jugador mire al enemigo y viceversa
        _enemySpirithar.transform.LookAt(_playerSpiritharPos);
        playerSpirithar.transform.LookAt(_enemyPosition);

        // Posicionar al jugador detr�s de su Spirithar utilizando el offset (convertido de local a global).
        Vector3 globalPlayerOffset = playerSpirithar.transform.TransformDirection(PlayerOffset);
        Player.transform.position = playerSpirithar.transform.position + globalPlayerOffset;
        Player.transform.LookAt(_enemyPosition);

        Debug.Log("Combate iniciado: " + playerSpirithar.spiritharName + " vs " + _enemySpirithar.spiritharName);

        // Display Name of Spirithars
        DisplayNameSpirithars(playerSpirithar);

        // Display moves of Player Spirithar
        DisplayMovesSpirithars(playerSpirithar);

    }

    private void DisplayNameSpirithars(Spirithar playerSpirithar) {
        PlayerCombatCaptureHUD.SetUpCaptureCombatHUD(playerSpirithar);
        EnemyCombatCaptureHUD.SetUpCaptureCombatHUD(_enemySpirithar);
    }

    private void DisplayMovesSpirithars(Spirithar playerSpirithar) {
        FirstButtonAbility.SetUpTextAbilityButton(playerSpirithar, _enemySpirithar, playerSpirithar.moves[0]);
        SecondButtonAbility.SetUpTextAbilityButton(playerSpirithar, _enemySpirithar, playerSpirithar.moves[1]);
        ThirdButtonAbility.SetUpTextAbilityButton(playerSpirithar, _enemySpirithar, playerSpirithar.moves[2]);
    }


}
