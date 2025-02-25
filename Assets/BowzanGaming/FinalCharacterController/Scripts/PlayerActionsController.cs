using System;
using UnityEngine;

namespace BowzanGaming.FinalCharacterController {
    public class PlayerActionsController : MonoBehaviour {
        // Actions Events
        public static event Action OnThrowingPressed;
        public static event Action OnThrowingReleased;

        // vars to control thow action
        public bool IsCaptureBallInstantiate;
        public bool IsCaptureBallThrew;

        //references own scripts
        private PlayerActionsInput _playerActionsInput;

        [Header("References other scripts")]
        [SerializeField] private CaptureBallLauncher _captureBallLauncher;
        

        private void Awake() {
            _playerActionsInput = GetComponent<PlayerActionsInput>();
            IsCaptureBallInstantiate = false;
            IsCaptureBallThrew = true;
        }
        // Update is called once per frame
        private void Update() {

            if (_playerActionsInput.OnHoldThrowPressed && !IsCaptureBallInstantiate) {
                print("Dentro del invocar evento");
                OnThrowingPressed?.Invoke();

            } else if (_playerActionsInput.OnHoldThrowPressed && _captureBallLauncher.DrawTrajectoryBall && IsCaptureBallInstantiate) {
                print("Dentro de el nuevo else if");
                _captureBallLauncher.CalculateForceAndStartTrajectory();
                _captureBallLauncher.DrawTrajectory(_captureBallLauncher.launchPoint.position, _captureBallLauncher.TotalForce);

            }

            if(!_playerActionsInput.OnHoldThrowPressed && _playerActionsInput.Throw && IsCaptureBallInstantiate && !IsCaptureBallThrew) {
                print("Dentro del Lanzar objeto");
                IsCaptureBallThrew = true;
                OnThrowingReleased?.Invoke();
            }

        }

        public void SetIsCaptureBallInstantiateFalse() { IsCaptureBallInstantiate = false; }
        public void SetIsCaptureBallThrewFalse() { IsCaptureBallThrew = false; }
    }


}
