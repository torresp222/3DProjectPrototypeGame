using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera, _virtualCameraCombat;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TransitionBetweenPlayerCameras() {

        if (_virtualCamera.Priority > _virtualCameraCombat.Priority) {
            _virtualCamera.Priority -= 1;
            _virtualCameraCombat.Priority += 1;
        } else {
            _virtualCameraCombat.Priority -= 1;
            _virtualCamera.Priority += 1;
        }
    }
}
