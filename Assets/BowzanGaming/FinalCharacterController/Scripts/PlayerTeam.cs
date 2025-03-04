using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTeam : MonoBehaviour {
    [Header("Configuración del Equipo")]
    public int maxTeamSize = 3;
    public List<Spirithar> team = new List<Spirithar>();

    // Método para añadir un Spirithar al equipo.
    public bool AddSpirithar(Spirithar newSpirithar) {
        if (team.Count >= maxTeamSize) {
            Debug.Log("El equipo ya está lleno.");
            return false;
        }

        team.Add(newSpirithar);
        Debug.Log("Spirithar " + newSpirithar.spiritharName + " añadido al equipo.");
        // Aquí podrías actualizar la UI o guardar el estado.
        return true;
    }

    /// <summary>
    /// Devuelve el prefab del Spirithar activo, por ejemplo, el primero de la lista.
    /// </summary>
    public GameObject GetActiveSpiritharPrefab() {
        if (team.Count > 0) {
            //team[0].gameObject
            return team[0].gameObject;
        } else {
            Debug.LogError("No hay Spirithar en el equipo del jugador.");
            return null;
        }
    }
}
