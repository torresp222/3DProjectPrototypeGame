using UnityEngine;
using System;

public class CaptureBall : MonoBehaviour {
    // Evento que se disparará al capturar un Spirithar.
    // Se pasa el Spirithar capturado para que el CombatManager sepa con quién iniciar el combate.
    public static event Action<Spirithar> OnSpiritharCaptured;

    private void OnCollisionEnter(Collision collision) {
        // Comprobamos si el objeto colisionado tiene un componente Spirithar.
        Spirithar spirithar = collision.gameObject.GetComponent<Spirithar>();
        if (spirithar != null) {
            Debug.Log("Bola de captura ha impactado con: " + spirithar.spiritharName);

            // Invocamos el evento para iniciar el combate.
            OnSpiritharCaptured?.Invoke(spirithar);

            // Opcionalmente, destruye la bola para no seguir detectando colisiones.
            Destroy(gameObject);
        }
    }
}
