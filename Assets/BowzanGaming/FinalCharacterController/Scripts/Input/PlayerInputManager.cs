using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BowzanGaming.FinalCharacterController {
    [DefaultExecutionOrder(-3)]
    public class PlayerInputManager : MonoBehaviour {
        
        public static PlayerInputManager Instance;

        public PlayerControls PlayerControls {  get; private set; }

        [Header("PlayerSoulCombat input")]
        public PlayerSoulCombatInput PlayerSoulCombatInput;
        public PlayerActionsInput PlayerActionsInput;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            PlayerSoulCombatInput.enabled = false;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
            print("------HABILITADO--------");
        }

        private void OnDisable() {
            PlayerControls.Disable();
            print("------DESSSSSHABILITADO--------");
        }

    }


}
