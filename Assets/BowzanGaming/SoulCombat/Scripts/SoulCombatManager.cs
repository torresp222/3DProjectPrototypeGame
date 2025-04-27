using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum BattleSoulState { NONE, PLAY }
public class SoulCombatManager : MonoBehaviour
{
    public static SoulCombatManager Instance;

    [Header("Object References")]
    public GameObject Player; // Player GO
    public GameObject GoldenHologram;

    [Header("Combat Manager Capture references")]
    [SerializeField] private CombatManager _combatManager; // Reference to regular CombatManager
    public PlayerTeamTracker PlayerTeamTracker;

    [Header("Soul Combat UI")]
    [SerializeField] private GameObject _spiritharMenu;
    [SerializeField] private GameObject _UISoulCombat;
    [SerializeField] private GameObject _aimGO;
    public CombatCaptureHUD PlayerSoulCombatCaptureHUD;
    public CombatCaptureHUD EnemySoulCombatCaptureHUD;
    public SpiritharCaptureHUD FirstSpiritharButton;
    public SpiritharCaptureHUD SecondSpiritharButton;
    public SpiritharCaptureHUD ThirdSpiritharButton;
    

    [Header("State of combate capture")]
    public BattleSoulState State;


    private AbsorptionManager _absorptionManager;
    private PlayerTeam _playerTeam;
    private PlayerSoulCombatInput _playerSoulInput;
    private PlayerSoulCombatAndStats _playerSoulCombatAndStats;

    private Spirithar _boosSoulSpirithar;
    private SoulSpiritharAi _playerSoulAi;
    private List<SpiritharCaptureHUD> _spiritharCaptureHUDs;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _absorptionManager = Player.GetComponent<AbsorptionManager>();
        _playerTeam = Player.GetComponent<PlayerTeam>();
        _playerSoulInput = Player.GetComponent<PlayerSoulCombatInput>();
        _playerSoulCombatAndStats = Player.GetComponent<PlayerSoulCombatAndStats>();
        _spiritharCaptureHUDs = new List<SpiritharCaptureHUD> { FirstSpiritharButton, SecondSpiritharButton, ThirdSpiritharButton };

        _UISoulCombat.SetActive(false);

    }

    private void OnEnable() {
        CaptureBall.OnSpiritharSoulCaptured += StartBossCombat;
        Spirithar.OnTakeDamage += UpdateSpiritharHPSlider;
        PlayerSoulCombatAndStats.OnTakeDamage += UpdateSpiritharHPSlider;

    }

    private void OnDisable() {
        CaptureBall.OnSpiritharSoulCaptured -= StartBossCombat;
        Spirithar.OnTakeDamage -= UpdateSpiritharHPSlider;
        PlayerSoulCombatAndStats.OnTakeDamage -= UpdateSpiritharHPSlider;
    }

    private void Update() {
        if (_playerSoulInput.SpiritharMenuOpen && State == BattleSoulState.PLAY) {
            // Open Menu
            _spiritharMenu.SetActive(true);
        } else {
            //close Menu
            _spiritharMenu.SetActive(false);
        }

    }

    public void StartBossCombat(Spirithar spirithar) {
        if (PlayerTeamTracker.CheckFirstSpiritharWithHealth() < 0) {
            Debug.Log("O no tienes Spirithars o estan todos SIN vida");
            return;
        }
        _boosSoulSpirithar = spirithar;
        _playerSoulAi = _boosSoulSpirithar.GetComponent<SoulSpiritharAi>();
        _boosSoulSpirithar.CurrentStateMode = EnemyState.Combat;
        State = BattleSoulState.PLAY;
        _UISoulCombat.SetActive(true);
        GoldenHologram.SetActive(false);
        // Disable Simple Action Control and Enable Soul Combat Inputs
        DisableNormalControls();
        InitializePlayerAbsorption();
        InitializeCombatSystem();
        SetSpiritharMenu();
        DisplayUISoulCombat();
        // Disable regular combat system
        if (_combatManager != null) _combatManager.enabled = false;
        _aimGO.SetActive(true);

    }

    public void SetSpiritharMenu() {
        // Recorrer todos los elementos del HUD
        for (int i = 0; i < _spiritharCaptureHUDs.Count; i++) {
            Spirithar teamMember = _playerTeam.team[i];
            _spiritharCaptureHUDs[i].SetUpTextSpiritharNameButton(teamMember, i);
        }
    }
    private void DisplayUISoulCombat() {
        PlayerSoulCombatCaptureHUD.SetInitializeHP(_playerSoulCombatAndStats.maxHealth);
        // PlayerSoulCombatCaptureHUD.SetUpCaptureCombatHUD(playerSpirithar);
        EnemySoulCombatCaptureHUD.SetUpCaptureCombatHUD(_boosSoulSpirithar);
    }

    public void UpdateSpiritharHPSlider(CombatMode combatMode) {
        if (combatMode == CombatMode.TurnBased) {
            return;
        }
        //PlayerTeamTracker.UpdateSpiritharHealthTeamTracked(_playerSpirithar.currentHealth, _currentSpiritharIndex);
        PlayerSoulCombatCaptureHUD.SetHP(_playerSoulCombatAndStats.currentHealth);
        EnemySoulCombatCaptureHUD.SetHP(_boosSoulSpirithar.currentHealth);

    }


    private void DisableNormalControls() {
        // Disable normal movement controls
        var normalControls = Player.GetComponent<PlayerActionsInput>();
        if (normalControls != null) normalControls.enabled = false;
    }

    private void InitializePlayerAbsorption() {
        // Force absorption of first team Spirithar (index 0)
        if (_playerTeam != null && _playerTeam.TeamCount() > 0) {
            _absorptionManager.AbsorbTeamSpirithar(0);
        }
    }

    private void InitializeCombatSystem() {

        // transition betweem virtual cameras
        CinemachineManager.Instance.TransitionBetweenPlayerCameras();

        // Enable combat-specific components;
        if (_playerSoulInput != null) _playerSoulInput.enabled = true;

        Debug.Log("Soul combat system activated");
    }

    public void EndBossCombat() {
        // transition betweem virtual cameras
        CinemachineManager.Instance.TransitionBetweenPlayerCameras();

        // Restore regular systems
        if (_combatManager != null) _combatManager.enabled = true;
        // Restore normal gameplay state
        var normalControls = Player.GetComponent<PlayerActionsInput>();
        if (normalControls != null) normalControls.enabled = true;

        //if (_playerSoulInput != null) _playerSoulInput.enabled = false;

        _absorptionManager.SetAbsorbedSpiritharToNone();
        _boosSoulSpirithar.CurrentStateMode = EnemyState.Idle;
        State = BattleSoulState.NONE;
        _UISoulCombat.SetActive(false);
        _aimGO.SetActive(false);
        /*_spiritharMenu.SetActive(false);*/

        Debug.Log("Soul combat system deactivated");

        if (GameManager.Instance != null) {
            StartCoroutine(GameManager.Instance.GoldenSpiritharDefeated());
        } else {
            Debug.LogError("No se pudo encontrar el GameManager para reportar la victoria.");
            // Como fallback, podrías cargar la escena directamente aquí, pero es menos limpio
            // SceneManager.LoadScene("MainMenu"); // Usa el nombre real de tu escena
        }
    }

    public void ChangeSpirithatAbsorbed(int index) {
        if(_absorptionManager.GetCurrenAbsorbSpiritharIndex == index && _playerTeam.GetActiveSpiritharIndex() == index) {
            print("Spirithar ya absorbido");
            return;

        }
        if(_playerTeam.team[index] == null)
            return;

        Debug.Log($"Absorbemos al nuevo spirithar del equipo {_playerTeam.team[index].spiritharName} posición en el equipo {index + 1}");
        _absorptionManager.AbsorbTeamSpirithar(index);
    }

    

}
