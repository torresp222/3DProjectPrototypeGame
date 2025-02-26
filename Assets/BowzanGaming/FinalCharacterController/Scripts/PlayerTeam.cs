using System.Collections.Generic;
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
}
