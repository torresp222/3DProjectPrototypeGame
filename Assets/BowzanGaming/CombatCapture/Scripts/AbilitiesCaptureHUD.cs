using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesCaptureHUD : MonoBehaviour 
{
    [Header("Abilities Button References")]
    public Button Button;

    [Header("Combat Manager references")]
    public CombatManager CombatManager;

    private Spirithar _playerSpirithar;
    private Spirithar _enemySpirithar;
    private SpiritharMove _spiritharMove;
    private TextMeshProUGUI _textAbility;

    public bool PerformingMove2;

    private void Awake() {
        _textAbility = Button.GetComponentInChildren<TextMeshProUGUI>();
        _textAbility.text = "Pruebaaa";
        //SetUpTextAbilityButton("Hooolaaa");
    }

    public void SetUpTextAbilityButton(Spirithar playerSpirithar, Spirithar enemySpirithar, SpiritharMove spiritharMove) {
        _playerSpirithar = playerSpirithar;
        _enemySpirithar = enemySpirithar;
        _spiritharMove = spiritharMove;
        _textAbility.text = _spiritharMove.moveName;
    }

    public void AbilityMove() {

        if (CombatManager.State != BattleCaptureState.PLAYERTURN || _playerSpirithar.PerformingMove)
            return;
        // No sé si hacerlo aquí o en CombatManager y si hacerlo o intentar con un Action
        // Pero pasarle a los button una función en el OnClick que sea dinámica. Según el Move que se le pase (lo tenemos en _spiritharMove) hacer el ataque la subida de defensa o subida de poder
        _playerSpirithar.PerformMove(_spiritharMove, _enemySpirithar); // On click del button hace el performove del spirithar (algo así hay que darle alguna vuelta)

        // Luego manejar los estados de los turnos ESO SI QUE SI EN EL COMBAT MANAGER. Invocar algo para que combat manager maneje los turnos y alomejor compruebe las muertes y demas
    }


}
