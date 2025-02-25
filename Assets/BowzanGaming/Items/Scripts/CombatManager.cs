using UnityEngine;

public class CombatManager : MonoBehaviour {
    private void OnEnable() {
        CaptureBall.OnSpiritharCaptured += InitiateCombat;
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharCaptured -= InitiateCombat;
    }

    // Este método se llamará cuando se capture un Spirithar.
    private void InitiateCombat(Spirithar capturedSpirithar) {
        Debug.Log("Iniciando combate con: " + capturedSpirithar.spiritharName);

        // Aquí es donde implementas la transición:
        // - Puedes activar/desactivar cámaras.
        // - Reposicionar al jugador y al enemigo.
        // - Cargar otra escena, o cambiar el modo de la escena actual.
        // Por el momento, este ejemplo solo registra el inicio en la consola.

        // Ejemplo de activación de una cámara de combate:
        // Camera.main.gameObject.SetActive(false);
        // combatCamera.SetActive(true);
    }
}
