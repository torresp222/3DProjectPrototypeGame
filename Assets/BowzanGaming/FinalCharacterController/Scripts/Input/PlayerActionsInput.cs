using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace BowzanGaming.FinalCharacterController {
    [DefaultExecutionOrder(-2)]
    public class PlayerActionsInput : MonoBehaviour, PlayerControls.IPlayerActionMapActions {
        #region Class Variables
        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private PlayerActionsController _playerActionsController;
        private PlayerSoulCombatInput _playerSoulCombatInput;
        public bool AttackPressed { get; private set; }
        public bool GatherPressed { get; private set; }
        public bool ThrowPressed { get; private set; }
        public bool OnHoldThrowPressed {  get; private set; }
        public bool Throw { get; private set; }
        #endregion
        #region Startup
        private void Awake() {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _playerActionsController = GetComponent<PlayerActionsController>();
            _playerSoulCombatInput = GetComponent<PlayerSoulCombatInput>();
            Throw = false;
        }
        private void OnEnable() {

            if (PlayerInputManager.Instance?.PlayerControls == null) {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.SetCallbacks(this);
        }

        private void OnDisable() {

            if (PlayerInputManager.Instance?.PlayerControls == null) {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.RemoveCallbacks(this);
        }


        #endregion
        #region Update Logic
        private void Update() {
            if(_playerLocomotionInput.MovementInput != Vector2.zero ||
               _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping ||
               _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling) {

                GatherPressed = false;
            }
        }

        public void SetGatherPressedFalse() { GatherPressed = false; }
        public void SetAttackPressedFalse() { AttackPressed = false; }
        public void SetThrowPressedFalse() { ThrowPressed = false; /*_playerActionsController.IsCaptureBallInstantiate = true;*/ }
        public void SetIsCaptureBallFalse() { _playerActionsController.IsCaptureBallInstantiate = false; }
        public void SetThrowFalse() { Throw = false; }

        #endregion
        #region Input Callbacks
        public void OnAttack(InputAction.CallbackContext context) {
            if(!context.performed)
                return;

            if (OnHoldThrowPressed) return;
            if (Throw) return;

            AttackPressed = true;
        }

        public void OnGather(InputAction.CallbackContext context) {
            if (!context.performed)
                return;

            GatherPressed = true;
        }

        public void OnThrow(InputAction.CallbackContext context) {
            if (!context.performed)
                return;

            if(AttackPressed) return;

            ThrowPressed = true;

        }

        public void OnHoldThrow(InputAction.CallbackContext context) {
            /*if (!context.performed)
                return;*/

            if (AttackPressed) return;

            if (context.performed){
                Throw = false;
                OnHoldThrowPressed = true;
            }

            if (context.canceled) {
                OnHoldThrowPressed = false;
                Throw = true;
            }
                
        }
        #endregion
    }
}



