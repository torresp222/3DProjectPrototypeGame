using UnityEngine;
using System;

public class CaptureBall : MonoBehaviour {
    // Evento que se disparará al capturar un Spirithar.
    // Se pasa el Spirithar capturado para que el CombatManager sepa con quién iniciar el combate.
    public static event Action<Spirithar> OnSpiritharCaptured;
    public static event Action OnSpiritharSoulCaptured;

    private void OnCollisionEnter(Collision collision) {
        // Comprobamos si el objeto colisionado tiene un componente Spirithar.
        Spirithar spirithar = collision.gameObject.GetComponent<Spirithar>();
        // Obtener el GameObject que colisionó
        GameObject collidedObject = collision.gameObject; // Usa collision.gameObject

        // Debug para verificar la colisión
        Debug.Log("Colisión con: " + collidedObject.name + " | Tag: " + collidedObject.tag);

        if (collidedObject.CompareTag("SoulSpirithar")) {
            Debug.Log("¡Colisión con SoulSpirithar!");
            OnSpiritharSoulCaptured?.Invoke();
            // Opcionalmente, destruye la bola para no seguir detectando colisiones.
            Destroy(gameObject);
        }

        if (spirithar != null && !collidedObject.CompareTag("SoulSpirithar")) {
            Debug.Log("Bola de captura ha impactado con: " + spirithar.spiritharName);

            // Invocamos el evento para iniciar el combate.
            OnSpiritharCaptured?.Invoke(spirithar);

            // Opcionalmente, destruye la bola para no seguir detectando colisiones.
            Destroy(gameObject);
        }
    }
}
