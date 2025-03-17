using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


namespace BowzanGaming.FinalCharacterController {
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        public float RotationMismatch { get; private set; } = 0f;
        public bool IsRotatingTotarget { get; private set; } = false;

        [Header("Base Movement")]
        public float walkAcceleration = 0.15f;
        public float walkSpeed = 3f;
        public float runAcceleration = 0.25f;
        public float runSpeed = 6f;
        public float sprintAcceleration = 0.5f;
        public float sprintSpeed = 9f;
        public float inAirAcceleration = 0.15f;
        public float drag = 0.1f;
        public float gravity = 25f;
        public float terminalVelocity = 50f;
        public float jumpSpeed = 1.0f;
        public float movingThreshold = 0.01f;

        [Header("Animation")]
        public float playerModelRotationSpeed = 10f;
        public float rotateToTargetTime = 0.25f;
        

        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        [Header("Camera Settings")]
        [SerializeField] private LayerMask _groundLayers;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerActionsInput _playerActionInput;
        private PlayerSoulCombatInput _playerSoulCombatInput;
        private PlayerState _playerState;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRoation = Vector2.zero;

        private bool _jumpedLastFrame = false;
        private bool _isRotatingClockWise = false;
        private float _rotatingToTargetTimer = 0f;
        private float _verticalVelocity = 0f;
        private float _antiBump;
        private float _stepOffset;

        private bool[] _actionsPlayerForRotatingToCameraView;

        private PlayerMovementState _lastMovementState = PlayerMovementState.Falling;
        #endregion

        #region Startup
        private void Awake() {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerActionInput = GetComponent<PlayerActionsInput>();
            _playerSoulCombatInput = GetComponent<PlayerSoulCombatInput>();
            _playerState = GetComponent<PlayerState>();
            

            _antiBump = sprintSpeed;
            _stepOffset = _characterController.stepOffset;
        }
        #endregion

        #region Update Logic
        private void Update() {

            UpdateMovementState();
            HandleVerticalMovement();
            HandleLateralMovement();
            _actionsPlayerForRotatingToCameraView = new bool[] { _playerActionInput.AttackPressed, _playerActionInput.OnHoldThrowPressed, _playerSoulCombatInput.SpellPressed };

        }

        private void UpdateMovementState() {
            _lastMovementState = _playerState.CurrentPlayerMovementState;

            bool canRun = CanRun();
            bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero; // order
            bool isMovingLaterally = IsMovingLaterally();                               // matter
            bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally; // order matters
            bool isWalking = (isMovingLaterally && !canRun)|| _playerLocomotionInput.WalkToggledOn;
            bool isGrounded = IsGrounded();


            PlayerMovementState lateralState = isWalking ? PlayerMovementState.Walking :
                                               isSprinting ? PlayerMovementState.Sprinting :
                                               isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

            _playerState.SetPlayerMovementState(lateralState);

            // Control Airborn State
            if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y >= 0f) {
                _playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
                _jumpedLastFrame = false;
                _characterController.stepOffset = 0f;

            } else if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y < 0f) {
                _playerState.SetPlayerMovementState(PlayerMovementState.Falling);
                _jumpedLastFrame = false;
                _characterController.stepOffset = 0f;

            } else {
                _characterController.stepOffset = _stepOffset;
            }

        }

        private void HandleVerticalMovement() {
            bool isGrounded = _playerState.IsGroundedState();

            _verticalVelocity -= gravity * Time.deltaTime;

            if (isGrounded && _verticalVelocity < 0 ) {
                _verticalVelocity = -_antiBump;
            }

            

            if (_playerLocomotionInput.JumpPressed && isGrounded) {
                _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
                _jumpedLastFrame = true;
            }

            if (_playerState.IsStateGroundedState(_lastMovementState) && !isGrounded) {
                _verticalVelocity += _antiBump;
            }

            if (Mathf.Abs(_verticalVelocity) > MathF.Abs(terminalVelocity)) {
                _verticalVelocity = -1 * MathF.Abs(terminalVelocity);
            }

        
        }

        private void HandleLateralMovement() {
            // Create quick references for current states
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isGrounded = _playerState.IsGroundedState();
            bool isWalking = _playerState.CurrentPlayerMovementState == PlayerMovementState.Walking;

            // State dependent acceleration and speed
            float lateralAcceleration = !isGrounded ? inAirAcceleration :
                                        isWalking ? walkAcceleration :
                                        isSprinting ? sprintAcceleration : runAcceleration;
            float clampLateralMagnitude = !isGrounded ? sprintSpeed :
                                          isWalking ? walkSpeed :
                                          isSprinting ? sprintSpeed : runSpeed;

            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;


            Vector3 movementDelta = movementDirection * lateralAcceleration;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            // Add drag to player
            Vector3 currentDrag = newVelocity.normalized * drag;
            newVelocity = (newVelocity.magnitude > drag) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);
            newVelocity.y += _verticalVelocity;
            newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;

            // Move character (Unity suggest only call this once per frame)
            _characterController.Move(newVelocity * Time.deltaTime);
        }

        private Vector3 HandleSteepWalls(Vector3 velocity) {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= _characterController.slopeLimit;

            if (!validAngle && _verticalVelocity < 0f) {
                velocity = Vector3.ProjectOnPlane(velocity, normal);
            }
            return velocity;
        }
        #endregion

        #region Late Update Logic
        private void LateUpdate() {
            UpdateCameraRotation();
        }

        private void UpdateCameraRotation() {
            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRoation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;
            
            float rotationTolerance = 90f;
            bool isPlayingActionRotatePlayer = _actionsPlayerForRotatingToCameraView.Any(action => action);
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            IsRotatingTotarget = _rotatingToTargetTimer > 0;
            // ROTATE if we're not idling
            if (!isIdling || isPlayingActionRotatePlayer) {
                RotatePlayerToTarget();
            }
            // If roation mismatch not within tolerance, or rotate to target is active, ROTATE
            else if (Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingTotarget) {
               UpdateIdleRotation(rotationTolerance);
            }

            

            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

            // Get angle between camera and player
            Vector3 cameraForwardProjectedXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 crossProduct = Vector3.Cross(transform.forward, cameraForwardProjectedXZ);
            float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
            RotationMismatch = sign * Vector3.Angle(transform.forward, cameraForwardProjectedXZ);
           
        }

        private void UpdateIdleRotation(float rotationTolerance) {
            // Initiate new rotation direction
            if (Mathf.Abs(RotationMismatch) > rotationTolerance) {
                _rotatingToTargetTimer = rotateToTargetTime;
                _isRotatingClockWise = RotationMismatch > rotationTolerance;
            }
            _rotatingToTargetTimer -= Time.deltaTime;
            Debug.Log("Rotating to Target Timer " + _rotatingToTargetTimer);

            // Rotate Player
            if (_isRotatingClockWise && RotationMismatch > 0f ||
                !_isRotatingClockWise && RotationMismatch < 0f) {
                Debug.Log("Entrooooo en roootaaar PLAYER ");
                RotatePlayerToTarget();
            }

            

            
        }

        private void RotatePlayerToTarget() {
            Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRoation.x, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
        }

        #endregion

        #region State Checks
        private bool IsMovingLaterally() {
            Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.y);

            return lateralVelocity.magnitude > movingThreshold;
        }
        private bool IsGrounded() {
            bool grounded = _playerState.IsGroundedState() ? IsGroudedWhileGrounded() : IsGroundedWhileAirborne();

            return grounded;
        }

        private bool IsGroudedWhileGrounded() {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _characterController.radius, transform.position.z);
            bool isGrounded = Physics.CheckSphere(spherePosition, _characterController.radius, _groundLayers, QueryTriggerInteraction.Ignore);
            return isGrounded;
        }

        private bool IsGroundedWhileAirborne() {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
            float angle = Vector3.Angle(normal, Vector3.up);
            print(angle);
            bool validAngle = angle <= _characterController.slopeLimit;

            return _characterController.isGrounded && validAngle;
        }
        private bool CanRun() {
            // This means player is moving diagonally ad 45 degrees or forward, if so we can run
            return _playerLocomotionInput.MovementInput.y >= Mathf.Abs(_playerLocomotionInput.MovementInput.x);
        }
        #endregion

    }
}

