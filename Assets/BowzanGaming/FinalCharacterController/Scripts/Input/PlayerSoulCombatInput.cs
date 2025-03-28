using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BowzanGaming.FinalCharacterController {
    public class PlayerSoulCombatInput : MonoBehaviour, PlayerControls.IPlayerSoulCombatMapActions {

        // PlayerSoulCombatInput.cs modifications
        // Add reference to combat handler
        private PlayerSoulCombatHandler _combatPlayerHandler;
        [SerializeField] private SoulCombatManager _soulCombatManager;

        // Variables para saber qué acción se ha pulsado
        public bool SpellPressed { get; private set; }
        public bool BoostDefensePressed { get; private set; }
        public bool BoostAttackPressed { get; private set; }
        public bool SpiritharMenuOpen { get; private set; }
        public int SpiritharSelected { get; private set; }

        private void Awake() {
            _combatPlayerHandler = GetComponent<PlayerSoulCombatHandler>();
        }

        private void OnEnable() {
            if (PlayerInputManager.Instance?.PlayerControls == null) {
                Debug.LogError("Player controls no inicializado en modo soul combat.");
                return;
            }
            // Activamos el nuevo Action Map
            PlayerInputManager.Instance.PlayerControls.PlayerSoulCombatMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerSoulCombatMap.SetCallbacks(this);
        }
        private void OnDisable() {
            if (PlayerInputManager.Instance?.PlayerControls == null) return;
            PlayerInputManager.Instance.PlayerControls.PlayerSoulCombatMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerSoulCombatMap.RemoveCallbacks(this);
        }

        public void SetSpellPressedFalse() { SpellPressed = false; }
        public void SetBoostAttackDefensePressedFalse() { BoostAttackPressed = false; BoostDefensePressed = false; }

        // Callbacks para cada acción
        public void OnSpell(InputAction.CallbackContext context) {
            if (!context.performed) return;
            if(BoostAttackPressed) return;
            if (BoostDefensePressed) return;
            SpellPressed = true;
            /*_combatHandler.PerformSpell();*/
            Debug.Log("Spell pressed");
        }

        public void OnBoostDefense(InputAction.CallbackContext context) {
            if (!context.performed) return;
            if (SpellPressed) return;
            if (BoostAttackPressed) return;
            BoostDefensePressed = true;
            _combatPlayerHandler.PerformDefenseBoost();
            Debug.Log("Boost Defense pressed");
        }

        public void OnBoostAttack(InputAction.CallbackContext context) {
            if (!context.performed) return;
            if (SpellPressed) return;
            if (BoostDefensePressed) return;
            BoostAttackPressed = true;
            _combatPlayerHandler.PerformAttackBoost();
            Debug.Log("Boost Attack pressed");
        }

        public void OnOpenSpiritharMenu(InputAction.CallbackContext context) {
            if (!context.performed) return;

            SpiritharMenuOpen = !SpiritharMenuOpen;
        }

        public void OnSelectSpiritharOne(InputAction.CallbackContext context) {
            if (!context.performed) return;
            if(!SpiritharMenuOpen) return;

            _soulCombatManager.ChangeSpirithatAbsorbed(0);
        }

        public void OnSelectSpirithar2(InputAction.CallbackContext context) {
            if (!context.performed) return;
            if (!SpiritharMenuOpen) return;
            _soulCombatManager.ChangeSpirithatAbsorbed(1);
        }

        public void OnSelectSpirithar3(InputAction.CallbackContext context) {
            if (!context.performed) return;
            if (!SpiritharMenuOpen) return;
            _soulCombatManager.ChangeSpirithatAbsorbed(2);
        }
    }

}
   
