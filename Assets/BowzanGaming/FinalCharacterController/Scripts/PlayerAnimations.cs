using System.Linq;
using UnityEngine;


namespace BowzanGaming.FinalCharacterController {

    public class PlayerAnimations : MonoBehaviour {

        [SerializeField] private Animator _animator;
        [SerializeField] private float _locomotionBlendSpeed = 0.02f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private PlayerController _playerController;
        private PlayerActionsInput _playerActionsInput;
        private PlayerSoulCombatInput _playerSoulCombatInput;

        //Locomotion
        private static int inputXHash = Animator.StringToHash("InputX");
        private static int inputYHash = Animator.StringToHash("InputY");
        private static int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");
        private static int isIdlingHash = Animator.StringToHash("isIdling");
        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");

        //Actions
        private static int isAttackingHash = Animator.StringToHash("isAttacking");
        private static int isThrowingHash = Animator.StringToHash("isThrowing");
        private static int isGatheringHash = Animator.StringToHash("isGathering");
        private static int isPlayingActionHash = Animator.StringToHash("isPlayingAction");
        private static int isHoldingThrowHash = Animator.StringToHash("isHoldingThrow");
        private static int isThrewHash = Animator.StringToHash("isThrew");
        private int[] actionHashes;

        //Actions CombatSoul
        private static int isSpellingHash = Animator.StringToHash("isSpelling");
        private static int isBoostingDefenseHash = Animator.StringToHash("isBoostingDefense");
        private static int isBoostingAttackHash = Animator.StringToHash("isBoostingAttack");

        //Camera/Rotation
        private static int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
        private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");


        private Vector3 _currentBlendInput = Vector3.zero;

        private float _sprintMaxBlendValue = 1.5f;
        private float _runMaxBlendValue = 1.0f;
        private float _valkMaxBlendValue = 0.5f;

        private void Awake() {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _playerController = GetComponent<PlayerController>();
            _playerActionsInput = GetComponent<PlayerActionsInput>();
            _playerSoulCombatInput = GetComponent<PlayerSoulCombatInput>(); 

            actionHashes = new int[] { isGatheringHash };
        }

        // Update is called once per frame
        private void Update() {
            UpdateAnimationState();

        }

        private void UpdateAnimationState() {
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            bool isSprinting  = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            bool isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            bool isGrounded = _playerState.IsGroundedState();
            bool isPlayingAction = actionHashes.Any(hash => _animator.GetBool(hash));

            bool isRunBlendValue = isRunning || isJumping || isFalling;
            Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * _sprintMaxBlendValue :
                                  isRunBlendValue ? _playerLocomotionInput.MovementInput * _runMaxBlendValue : _playerLocomotionInput.MovementInput * _valkMaxBlendValue;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, _locomotionBlendSpeed * Time.deltaTime);

            _animator.SetBool(isGroundedHash, isGrounded);
            _animator.SetBool(isIdlingHash, isIdling);
            _animator.SetBool(isFallingHash, isFalling);
            _animator.SetBool(isJumpingHash, isJumping);
            _animator.SetBool(isRotatingToTargetHash, _playerController.IsRotatingTotarget);
            _animator.SetBool(isAttackingHash, _playerActionsInput.AttackPressed /*|| _playerSoulCombatInput.SpellPressed*/);
            _animator.SetBool(isGatheringHash, _playerActionsInput.GatherPressed);
            _animator.SetBool(isThrowingHash, _playerActionsInput.ThrowPressed);
            _animator.SetBool(isPlayingActionHash, isPlayingAction);

            _animator.SetBool(isSpellingHash, _playerSoulCombatInput.SpellPressed);
            _animator.SetBool(isBoostingDefenseHash, _playerSoulCombatInput.BoostDefensePressed);
            _animator.SetBool(isBoostingAttackHash, _playerSoulCombatInput.BoostAttackPressed);

            _animator.SetBool(isHoldingThrowHash, _playerActionsInput.OnHoldThrowPressed);
            _animator.SetBool(isThrewHash, _playerActionsInput.Throw);

            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
        }
    }

}

