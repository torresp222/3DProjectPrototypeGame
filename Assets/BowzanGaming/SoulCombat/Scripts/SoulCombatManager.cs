using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SoulCombatManager : MonoBehaviour
{
    public static SoulCombatManager Instance;

    [Header("Player References")]
    public GameObject Player; // Player GO

    [Header("Combat Manager Capture references")]
    [SerializeField] private CombatManager _combatManager; // Reference to regular CombatManager

    [Header("Soul Combat UI")]
    [SerializeField] private GameObject _spiritharMenu;
    public SpiritharCaptureHUD FirstSpiritharButton;
    public SpiritharCaptureHUD SecondSpiritharButton;
    public SpiritharCaptureHUD ThirdSpiritharButton;
    

    private AbsorptionManager _absorptionManager;
    private PlayerTeam _playerTeam;
    private PlayerSoulCombatInput _playerSoulInput;

    private List<SpiritharCaptureHUD> _spiritharCaptureHUDs;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _absorptionManager = Player.GetComponent<AbsorptionManager>();
        _playerTeam = Player.GetComponent<PlayerTeam>();
        _playerSoulInput = Player.GetComponent<PlayerSoulCombatInput>();
        _spiritharCaptureHUDs = new List<SpiritharCaptureHUD> { FirstSpiritharButton, SecondSpiritharButton, ThirdSpiritharButton };

    }

    private void OnEnable() {
        CaptureBall.OnSpiritharSoulCaptured += StartBossCombat;
        
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharSoulCaptured -= StartBossCombat;
    }

    private void Update() {
        if (_playerSoulInput.SpiritharMenuOpen) {
            // Open Menu
            _spiritharMenu.SetActive(true);
        } else {
            //close Menu
            _spiritharMenu.SetActive(false);
        }

    }

    public void StartBossCombat() {
        
        // Disable Simple Action Control and Enable Soul Combat Inputs
        DisableNormalControls();
        InitializePlayerAbsorption();
        InitializeCombatSystem();
        SetSpiritharMenu();
        // Disable regular combat system
        if (_combatManager != null) _combatManager.enabled = false;

    }

    public void SetSpiritharMenu() {
        // Recorrer todos los elementos del HUD
        for (int i = 0; i < _spiritharCaptureHUDs.Count; i++) {
            Spirithar teamMember = _playerTeam.team[i];
            _spiritharCaptureHUDs[i].SetUpTextSpiritharNameButton(teamMember, i);
        }
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

        // Enable combat-specific components
        var combatInput = Player.GetComponent<PlayerSoulCombatInput>();
        if (combatInput != null) combatInput.enabled = true;

        Debug.Log("Soul combat system activated");
    }

    public void EndBossCombat() {
        // Restore regular systems
        if (_combatManager != null) _combatManager.enabled = true;
        // Restore normal gameplay state
        var normalControls = Player.GetComponent<PlayerActionsInput>();
        if (normalControls != null) normalControls.enabled = true;

        var combatInput = Player.GetComponent<PlayerSoulCombatInput>();
        if (combatInput != null) combatInput.enabled = false;

        Debug.Log("Soul combat system deactivated");
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
