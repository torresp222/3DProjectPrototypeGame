using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BowzanGaming.FinalCharacterController {
    public class CaptureBallLauncher : MonoBehaviour {
        [Header("Lanzamiento")]
        public GameObject projectilePrefab; // Prefab del objeto a lanzar
        public Transform launchPoint;      // Punto desde donde se lanza
        public float launchForce = 10f;    // Fuerza del lanzamiento
        public float upwardForce = 5f;     // Fuerza hacia arriba para la parábola

        [Header("References")]
        [SerializeField] private PlayerActionsController _playerActionController;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private GameObject _playerTransform;
        public LineRenderer lineRenderer; // Asigna este componente en el inspector

        public Vector3 ForceDirection { get; private set; }
        public Vector3 TotalForce { get; private set; }

        public Rigidbody ProjectileRB { get; private set; }
        public bool DrawTrajectoryBall { get; private set; }

        private void Awake() {
            DrawTrajectoryBall = false;
        }

        /// <summary>
        /// Lanza un proyectil con una trayectoria parabólica.
        /// </summary>

        public void InstantiateProjectile() {
            if (projectilePrefab == null || launchPoint == null) {
                Debug.LogError("Falta configurar el prefab o el punto de lanzamiento.");
                return;
            }

            // Instanciar el objeto lanzado
            GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);
            // Hacer que el objeto sea hijo de launchPoint
            projectile.transform.SetParent(launchPoint);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            ProjectileRB = rb;

            DrawTrajectoryBall = true;
            _playerActionController.IsCaptureBallInstantiate = true;

        }

        public void CalculateForceAndStartTrajectory() {
            ForceDirection = _playerCamera.transform.forward; // Dirección hacia adelante
            TotalForce = ForceDirection * launchForce + Vector3.up * upwardForce;
      
        }

        public void LaunchProjectile() {
            if (ProjectileRB != null) {
                // Desparentar el objeto
                ProjectileRB.transform.SetParent(null);

                ProjectileRB.isKinematic = false; // Reactivar la física

                // Aplicar la fuerza al Rigidbody
                ProjectileRB.AddForce(TotalForce, ForceMode.Impulse);
                lineRenderer.enabled = false;
                DrawTrajectoryBall = false;
            } else {
                Debug.LogError("El prefab del proyectil no tiene un Rigidbody adjunto.");
            }

        }
        public void DrawTrajectory(Vector3 startPosition, Vector3 initialVelocity) {
            lineRenderer.enabled = true;
            int numSegments = 30;       // Número de puntos a calcular en la trayectoria
            float timeStep = 0.1f;      // Intervalo de tiempo entre cada punto
            Vector3[] trajectoryPoints = new Vector3[numSegments];

            for (int i = 0; i < numSegments; i++) {
                float t = i * timeStep;
                // Usamos la fórmula: P(t) = P0 + V0 * t + 0.5 * a * t^2
                Vector3 point = startPosition + initialVelocity * t + 0.5f * Physics.gravity * t * t;
                trajectoryPoints[i] = point;
            }

            // Configurar el LineRenderer con los puntos calculados
            lineRenderer.positionCount = numSegments;
            lineRenderer.SetPositions(trajectoryPoints);
        }
        private void OnEnable() {
            PlayerActionsController.OnThrowingPressed += InstantiateProjectile;
            PlayerActionsController.OnThrowingReleased += LaunchProjectile;

        }
        private void OnDisable() {
            PlayerActionsController.OnThrowingPressed -= InstantiateProjectile;
            PlayerActionsController.OnThrowingReleased -= LaunchProjectile;

        }
    }
}
