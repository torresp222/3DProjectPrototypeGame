using BowzanGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BowzanGaming.FinalCharacterController {
    public class PlayerSoulCombatInput : MonoBehaviour, PlayerControls.IPlayerSoulCombatMapActions {
        // Variables para saber qué acción se ha pulsado
        public bool SpellPressed { get; private set; }
        public bool BoostDefensePressed { get; private set; }
        public bool BoostAttackPressed { get; private set; }

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

        // Callbacks para cada acción
        public void OnSpell(InputAction.CallbackContext context) {
            if (!context.performed) return;
            SpellPressed = true;
            Debug.Log("Spell pressed");
        }

        public void OnBoostDefense(InputAction.CallbackContext context) {
            if (!context.performed) return;
            BoostDefensePressed = true;
            Debug.Log("Boost Defense pressed");
        }

        public void OnBoostAttack(InputAction.CallbackContext context) {
            if (!context.performed) return;
            BoostAttackPressed = true;
            Debug.Log("Boost Attack pressed");
        }
    }

}
   
