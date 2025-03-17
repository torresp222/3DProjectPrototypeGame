using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SoulCombatManager : MonoBehaviour
{
    public static SoulCombatManager Instance;

    [Header("References")]
    public GameObject Player;           // Player GO
    [SerializeField] private MonoBehaviour _combatManager; // Reference to regular CombatManager

    private AbsorptionManager _absorptionManager;
    private PlayerTeam _playerTeam;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _absorptionManager = Player.GetComponent<AbsorptionManager>();
        _playerTeam = Player.GetComponent<PlayerTeam>();
    }

    private void OnEnable() {
        CaptureBall.OnSpiritharSoulCaptured += StartBossCombat;
        
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharSoulCaptured -= StartBossCombat;
    }

    public void StartBossCombat() {
        
        // Disable Simple Action Control and Enable Soul Combat Inputs
        DisableNormalControls();
        InitializePlayerAbsorption();
        InitializeCombatSystem();
        // Disable regular combat system
        if (_combatManager != null) _combatManager.enabled = false;

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

}
