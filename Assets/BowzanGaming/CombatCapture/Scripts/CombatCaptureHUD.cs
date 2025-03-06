using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatCaptureHUD : MonoBehaviour
{
    [Header("Ui Combat references")]
    public Image PlayerSpiritharInfo; // Componente Image que se pasa por inspector
    public Image EnemySpiritharInfo; // Componente Image que se pasa por inspector

    private TextMeshProUGUI _playerSpiritharName; // varibale donde se va a pasar el textmeshprogui
    private TextMeshProUGUI _playerSpiritharLvl; // varibale donde se va a pasar el textmeshprogui
    private TextMeshProUGUI _enemySpiritharname; // otra variable donde se va a pasar la otra textmeshprogui
    private TextMeshProUGUI _enemySpiritharLvl; // otra variable donde se va a pasar la otra textmeshprogui

    private void Awake() {
        // Buscar los TextMeshProUGUI por nombre dentro del Image (Prueba)
        _playerSpiritharName = PlayerSpiritharInfo.transform.Find("SpiritharName")?.GetComponent<TextMeshProUGUI>();
        _playerSpiritharLvl = PlayerSpiritharInfo.transform.Find("SpiritharLvl")?.GetComponent<TextMeshProUGUI>();
        _enemySpiritharname = EnemySpiritharInfo.transform.Find("SpiritharName")?.GetComponent<TextMeshProUGUI>();
        _enemySpiritharLvl = EnemySpiritharInfo.transform.Find("SpiritharLvl")?.GetComponent<TextMeshProUGUI>();

        // Verificar si se encontraron los componentes
        if (_playerSpiritharName == null) Debug.LogError("PlayerSpiritharName no encontrado");
        if (_playerSpiritharLvl == null) Debug.LogError("PlayerSpiritharLvl no encontrado");
        if (_enemySpiritharname == null) Debug.LogError("EnemySpiritharName no encontrado");
        if (_enemySpiritharLvl == null) Debug.LogError("EnemySpiritharLvl no encontrado");
    }

    // Set the names, Lvl, HP of the Spirithar in the ui combat capture
    public void SetUpCaptureCombat(Spirithar playerSpirithar, Spirithar enemySpirithar) {
        _playerSpiritharName.text = playerSpirithar.spiritharName;
        _enemySpiritharname.text= enemySpirithar.spiritharName;

        _playerSpiritharLvl.text = "Lvl " + playerSpirithar.Lvl.ToString();
        _enemySpiritharLvl.text = "Lvl " + enemySpirithar.Lvl.ToString();
    }
}
