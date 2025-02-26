using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour {
    [Header("Configuraci�n del Equipo")]
    public int maxTeamSize = 3;
    public List<Spirithar> team = new List<Spirithar>();

    // M�todo para a�adir un Spirithar al equipo.
    public bool AddSpirithar(Spirithar newSpirithar) {
        if (team.Count >= maxTeamSize) {
            Debug.Log("El equipo ya est� lleno.");
            return false;
        }

        team.Add(newSpirithar);
        Debug.Log("Spirithar " + newSpirithar.spiritharName + " a�adido al equipo.");
        // Aqu� podr�as actualizar la UI o guardar el estado.
        return true;
    }
}
