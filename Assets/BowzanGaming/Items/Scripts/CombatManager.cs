using UnityEngine;

public class CombatManager : MonoBehaviour {
    private void OnEnable() {
        CaptureBall.OnSpiritharCaptured += InitiateCombat;
    }

    private void OnDisable() {
        CaptureBall.OnSpiritharCaptured -= InitiateCombat;
    }

    // Este m�todo se llamar� cuando se capture un Spirithar.
    private void InitiateCombat(Spirithar capturedSpirithar) {
        Debug.Log("Iniciando combate con: " + capturedSpirithar.spiritharName);

        // Aqu� es donde implementas la transici�n:
        // - Puedes activar/desactivar c�maras.
        // - Reposicionar al jugador y al enemigo.
        // - Cargar otra escena, o cambiar el modo de la escena actual.
        // Por el momento, este ejemplo solo registra el inicio en la consola.

        // Ejemplo de activaci�n de una c�mara de combate:
        // Camera.main.gameObject.SetActive(false);
        // combatCamera.SetActive(true);
    }
}
