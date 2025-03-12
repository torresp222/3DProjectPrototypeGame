using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoulCombatManager : MonoBehaviour
{
    public static SoulCombatManager Instance;

    [Header("References")]
    public GameObject Player;            // Player GO

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable() {
        CaptureBall.OnSpiritharSoulCaptured += StartBossCombat;
        
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharSoulCaptured -= StartBossCombat;
    }

    public void StartBossCombat() {
        // Disable Simple Action Control and Enable Soul Combat Inputs
        DisablePlayerSimpleActionControl();

    }

    public void DisablePlayerSimpleActionControl() {
        PlayerActionsInput pai = Player.GetComponent<PlayerActionsInput>();
        if (pai != null) pai.enabled = false;

        PlayerSoulCombatInput psci = Player.GetComponent<PlayerSoulCombatInput>();
        if(psci != null) psci.enabled = true;

        Debug.Log("HABILITADO EL INPUT DE SOUL COMBAT");

    }

}
