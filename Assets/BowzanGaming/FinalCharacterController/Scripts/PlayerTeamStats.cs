using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerTeamStats : MonoBehaviour
{
    // Al instanciar en combate comprobar si ya ha sido instanciado el spirithar. Si ya ha sido instanciado no inicializar sus stats y demás/
    //Puede ser que en el PlayerTeam controlemos si se ha instanciado ya el Spirithar de cada posición, el combat manager pregunte si se ha instanciado.
    //Si es que NO, se inicializa sus stats y si es que si, se mantienen sus stats.
    //Como se mantienen? Podemos controlarlo con otro script en el juagdor PlayerTeamStats, que se le añada al coger los stats y cada vez que se instancie de nuevo (después de que se haya instanciado una vez)
    //se les pasa sus stats iniciales (que se han guardado en PlayerTeamStats). Cada vez que recibe dmg el spirithar tiene que guardarlo el PlayerTeamStats
    //en el lugar del spirithar para recibirlo luego si se instancia de nuevo.
    //También guarda los stats de Attack o tanky (vida máxima, daño y defensa).
}
