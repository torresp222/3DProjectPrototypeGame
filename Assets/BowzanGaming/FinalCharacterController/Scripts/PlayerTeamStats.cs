using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerTeamStats : MonoBehaviour
{
    // Al instanciar en combate comprobar si ya ha sido instanciado el spirithar. Si ya ha sido instanciado no inicializar sus stats y dem�s/
    //Puede ser que en el PlayerTeam controlemos si se ha instanciado ya el Spirithar de cada posici�n, el combat manager pregunte si se ha instanciado.
    //Si es que NO, se inicializa sus stats y si es que si, se mantienen sus stats.
    //Como se mantienen? Podemos controlarlo con otro script en el juagdor PlayerTeamStats, que se le a�ada al coger los stats y cada vez que se instancie de nuevo (despu�s de que se haya instanciado una vez)
    //se les pasa sus stats iniciales (que se han guardado en PlayerTeamStats). Cada vez que recibe dmg el spirithar tiene que guardarlo el PlayerTeamStats
    //en el lugar del spirithar para recibirlo luego si se instancia de nuevo.
    //Tambi�n guarda los stats de Attack o tanky (vida m�xima, da�o y defensa).
}
