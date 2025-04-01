using BowzanGaming.FinalCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static PlayerTeamTracker;
using Random = UnityEngine.Random;


public enum BattleCaptureState { START, PLAYERTURN, ENEMYTURN }

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
    public PlayerTeamTracker PlayerTeamTracker;
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

    [Header("Intern references to assign at the beggining of each combat")]
    // Referencias internas que se asignarán al iniciar el combate.
    [SerializeField] private Spirithar _enemySpirithar; // Spirithar salvaje capturado
    [SerializeField] private Vector3 _enemyPosition;
    private Spirithar _playerSpirithar; // Spirithar activo del equipo (instanciado en combate)
    private int _currentSpiritharIndex;
    private SpiritharMove _currentMove;
    private string[] _keysTeamTracker = { "spiritharOne", "spiritharTwo", "spiritharThree" };
    private GameObject _firstSpiritharTeam;// GameObject "" ""
    [SerializeField]private GameObject _currentSpiritharCombat;
    [SerializeField] private Vector3 _playerSpiritharPos;
    private int _numOfTurns = 2;
    private int _numTurn = 0;
    private bool _combatHasEnded;

    


    private void Awake() {
        // Patrón singleton para acceso global.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
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
        AbilitiesCaptureHUD.OnAbilityPress += SetStateTurnCombat;
        // Spirithar.OnEnemySpiritharNotDead += ChangeState;
        Spirithar.OnSpiritharDead += TeamHealthChecker;
        /*Spirithar.OnSpiritharDead += () => { EnablePlayerControl(); TeamHealthChecker(); };*/
        SpiritharCaptureHUD.OnSpiritharClickToChange += PlaceSpiritharTeamInCombat;
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharCaptured -= InitiateCombat;
        Spirithar.OnTakeDamage -= UpdateSpiritharHPSlider;
        AbilitiesCaptureHUD.OnAbilityPress -= SetStateTurnCombat;
        //Spirithar.OnEnemySpiritharNotDead -= ChangeState;
        Spirithar.OnSpiritharDead -= TeamHealthChecker;
        /*Spirithar.OnSpiritharDead -= () => { EnablePlayerControl(); TeamHealthChecker(); };*/
        SpiritharCaptureHUD.OnSpiritharClickToChange -= PlaceSpiritharTeamInCombat;
    }

    // Este método se llamará cuando se capture un Spirithar.
    private void InitiateCombat(Spirithar capturedSpirithar) {
        _combatHasEnded = false;
        if (PlayerTeamTracker.CheckFirstSpiritharWithHealth() < 0) {
            Debug.Log("O no tienes Spirithars o estan todos SIN vida");
            return;
        }
            
        Debug.Log("Iniciando combate con: " + capturedSpirithar.spiritharName);
        State = BattleCaptureState.START;
        if (AbilitiesMenu != null)
            AbilitiesMenu.SetActive(false);
        if (SpiritharMenu != null)
            SpiritharMenu.SetActive(false);

        CheckIfButtonInteractableAndSpiritharAlive();
        StartCombat(capturedSpirithar);
    }

    /// <summary>
    /// Inicia el combate, instanciando el Spirithar activo del equipo y posicionando a todos los participantes.
    /// </summary>
    /// <param name="capturedEnemySpirithar">El Spirithar salvaje capturado.</param>
    public void StartCombat(Spirithar capturedEnemySpirithar) {
        _currentSpiritharIndex = PlayerTeamTracker.CheckFirstSpiritharWithHealth();
        // Asignar el Spirithar salvaje y conservar su posición actual.
        _enemySpirithar = capturedEnemySpirithar;
        _enemySpirithar.SetFirstStats();
        _enemyPosition = _enemySpirithar.transform.position;

        // Instanciar el Spirithar activo del equipo a partir del prefab guardado en PlayerTeam.
        _firstSpiritharTeam = PlayerTeam.GetActiveSpiritharPrefab();
        if (_firstSpiritharTeam == null) {
            Debug.LogError("No se encontró un prefab de Spirithar en el equipo del jugador.");
            return;
        }

        // Place the spirithar of team in combat
        PlaceSpiritharTeamInCombat(_firstSpiritharTeam, _currentSpiritharIndex);
        
        // Set the menu of spirithars in team
        SetSpiritharMenu();

        // Deshabilitar controles y la cámara del jugador.
        DisablePlayerControl();

        // Activar la cámara de combate.
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

        //State = BattleCaptureState.PLAYERTURN;

       
    }

    public void SetSpiritharMenu() {
        // Recorrer todos los elementos del HUD
        for (int i = 0; i < _spiritharCaptureHUDs.Count; i++) {
            Spirithar teamMember = PlayerTeam.team[i];
            _spiritharCaptureHUDs[i].SetUpTextSpiritharNameButton(teamMember, i);
        }
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

        _playerSpiritharPos = Vector3.zero;
        PlayerTeam.SwitchActiveSpirithar(0);
        _combatHasEnded = true;
        State = BattleCaptureState.START;
        Destroy(_currentSpiritharCombat);
    }

    public void TeamHealthChecker() {
        if (PlayerTeamTracker.SpiritharStatsTracker.Values.Any(d => d.TrackCurrentHealth <= 0f && d.IsTracked)) {
            Debug.Log($"¡Algún Spirithar tiene salud 0! current INDEX {_currentSpiritharIndex}");
            if (PlayerTeamTracker.SpiritharStatsTracker.Values.Any(d => d.TrackCurrentHealth > 0f)) {
                Debug.Log("¡Algún Spirithar sigue vivo!");
                if (SpiritharMenu != null) {
                    CombatMenuGO.SetActive(false);
                    AbilitiesMenu.SetActive(false);
                    SpiritharMenu.SetActive(true);
                }
                SpiritharCaptureHUD spiritharButtonToDisable = GetChangeSpiritharButton(_currentSpiritharIndex);
                spiritharButtonToDisable.DisableButton();
                if (_enemySpirithar.currentHealth <= 0f) {
                    Debug.Log("ENEMY DEAD");
                    EnablePlayerControl();
                }
            } else {
                Debug.Log("Todos los del team dead dead");
                EnablePlayerControl();
            }
        } else {
            Debug.Log("Todos tus Spirithars Vivitos y Coleando");
            EnablePlayerControl();
        }
    }

    public SpiritharCaptureHUD GetChangeSpiritharButton(int index) {

        switch (index) {
            case 0:
                return FirstSpiritharButton;
            case 1:
                return SecondSpiritharButton;
            case 2:
                return ThirdSpiritharButton;
            default:
                Debug.LogError($"Índice {index} no válido");
                return null;
        }

    }

    private void CheckIfButtonInteractableAndSpiritharAlive() {
        SpiritharCaptureHUD buttonToCheck;
        for (int i = 0; i < PlayerTeamTracker.TrackedSlots.Length; i++) {
            buttonToCheck = GetChangeSpiritharButton(i);
            if (PlayerTeamTracker.SpiritharStatsTracker.TryGetValue(PlayerTeamTracker.TrackedSlots[i], out SpiritharData currentData)) {
                if (currentData.IsTracked && currentData.TrackCurrentHealth > 0) {
                    EnableInteractableSpiritharButton(buttonToCheck);
                }
            }
        }
    }

    public void EnableInteractableSpiritharButton(SpiritharCaptureHUD button) {
        button.EnableButton();
    }

    public void UpdateSpiritharHPSlider(CombatMode combatMode) {
        if (combatMode == CombatMode.Soul) {
            return;
        }
        PlayerTeamTracker.UpdateSpiritharHealthTeamTracked(_playerSpirithar.currentHealth, _currentSpiritharIndex);
        PlayerCombatCaptureHUD.SetHP(PlayerTeamTracker.SpiritharStatsTracker[_keysTeamTracker[_currentSpiritharIndex]].TrackCurrentHealth);
        Debug.Log(_enemySpirithar.currentHealth);
        EnemyCombatCaptureHUD.SetHP(_enemySpirithar.currentHealth);

    }

    private void SetStateTurnCombat(SpiritharMove move) {
        _currentMove = move;

        if (State == BattleCaptureState.START) {
            State = GetFastestSpiritharAndChangeState();
            Debug.Log($"El State seleccionado es {State}");
        } else
            return;

        ExecuteNextTurn(State);
        
    }

    private void ExecuteNextTurn(BattleCaptureState state) {

        if (_combatHasEnded)
            return;

        if (_numTurn >= _numOfTurns) {
            State = BattleCaptureState.START;
            _numTurn = 0;
            Debug.Log("Volvemos a elegir");
            return;
        }

        switch (state) {
            case BattleCaptureState.PLAYERTURN:
                _numTurn++;
                _playerSpirithar.PerformingMove = false;
                StartCoroutine(PlayerTurn(_currentMove));
                break;
            case BattleCaptureState.ENEMYTURN:
                _numTurn++;
                //_enemySpirithar.PerformingMove = false;
                StartCoroutine(EnemyTurn());
                break;


        }

    }
    private BattleCaptureState GetFastestSpiritharAndChangeState() {
        BattleCaptureState[] allowedStates = {
            BattleCaptureState.PLAYERTURN,
            BattleCaptureState.ENEMYTURN
        };
        BattleCaptureState randomState = allowedStates[Random.Range(0, allowedStates.Length)];

        return _playerSpirithar.baseStats.baseSpeed > _enemySpirithar.baseStats.baseSpeed ? BattleCaptureState.PLAYERTURN:
               _enemySpirithar.baseStats.baseSpeed > _playerSpirithar.baseStats.baseSpeed ? BattleCaptureState.ENEMYTURN :
               randomState;
    }
    /*public void ChangeState() {

        if (State == BattleCaptureState.PLAYERTURN) {
            State = BattleCaptureState.ENEMYTURN;
            _enemySpirithar.PerformingMove = false;
            StartCoroutine(EnemyTurn());
        } else 
            return;
    }*/

    public IEnumerator PlayerTurn(SpiritharMove spiritharMove) {
        Debug.Log("Turno del player");
        if (spiritharMove == null) {
            Debug.LogError($"The Enemy Spirithar {_enemySpirithar} has no abilities");
            yield break;
        }
        _playerSpirithar.PerformMove(spiritharMove, _enemySpirithar);
        yield return new WaitForSeconds(2f);
        State = BattleCaptureState.ENEMYTURN;
        _playerSpirithar.PerformingMove = false;
        ExecuteNextTurn(State);
        

    }

    public IEnumerator EnemyTurn() {
        Debug.Log("Turno del enemigo");
        SpiritharMove move = RandomizeManager.Instance.GetRandomMoveFrom(_enemySpirithar);
        if (move == null) {
            Debug.LogError($"The Enemy Spirithar {_enemySpirithar} has no abilities");
            yield break;
        }
        _enemySpirithar.PerformMove(move, _playerSpirithar, PlayerTeamTracker.SpiritharStatsTracker[_keysTeamTracker[_currentSpiritharIndex]].IsTracked);
        yield return new WaitForSeconds(2f);
        State = BattleCaptureState.PLAYERTURN;
        _enemySpirithar.PerformingMove = false;
        ExecuteNextTurn(State);
        

    }

    // Logic for changing and placing spirithar in turn based combat
    private void PlaceSpiritharTeamInCombat(GameObject spiritharGO, int index) {

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
        
        SpiritharData data = PlayerTeamTracker.SpiritharStatsTracker[_keysTeamTracker[index]];
        Debug.Log($"el indez es {index} y la key de trackeo es {data}");
        playerSpirithar.maxHealth = data.TrackBaseStat.baseHealth;
        playerSpirithar.currentHealth = data.TrackCurrentHealth;
        playerSpirithar.baseStats = data.TrackBaseStat;
        playerSpirithar.stats = new SpiritharStats(playerSpirithar.baseStats);
        _playerSpirithar = playerSpirithar;
        _currentSpiritharIndex = index;


        if (playerSpirithar == null) {
            Debug.LogError("El prefab instanciado no tiene el componente Spirithar.");
            return;
        }
        
        // Posicionar al _enemySpirithar: se mantiene en su posición actual.
        // Posicionar al playerSpirithar: se coloca a una distancia CombatDistance del enemigo, en dirección opuesta al frente del enemigo.
        
        playerSpirithar.transform.position = _playerSpiritharPos;
        // Hacer que el Spirithar del jugador mire al enemigo y viceversa
        _enemySpirithar.transform.LookAt(_playerSpiritharPos);
        playerSpirithar.transform.LookAt(_enemyPosition);

        // Posicionar al jugador detrás de su Spirithar utilizando el offset (convertido de local a global).
        Vector3 globalPlayerOffset = playerSpirithar.transform.TransformDirection(PlayerOffset);
        Player.transform.position = playerSpirithar.transform.position + globalPlayerOffset;
        Player.transform.LookAt(_enemyPosition);

        Debug.Log("Combate iniciado: " + playerSpirithar.spiritharName + " vs " + _enemySpirithar.spiritharName);

        //Check disable buttons and if it needs to be enable
        CheckDisableButtonsSpiritharMenu();

        // Display Name of Spirithars
        DisplayNameSpirithars(playerSpirithar);

        // Update health bar
        UpdateSpiritharHPSlider(CombatMode.TurnBased);

        // Display moves of Player Spirithar
        DisplayMovesSpirithars(playerSpirithar);

    }

    public void CheckDisableButtonsSpiritharMenu() {
        if (_spiritharCaptureHUDs.Any(d => d.Button.interactable == false && d.PlayerSpirithar.currentHealth > 0)){

        }
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
