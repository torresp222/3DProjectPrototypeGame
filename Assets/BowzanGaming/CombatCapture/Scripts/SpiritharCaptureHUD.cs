using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpiritharCaptureHUD : MonoBehaviour {

    public static event Action<GameObject, int> OnSpiritharClickToChange;

    [Header("Spirithar Button References")]
    public Button Button;

    /*[Header("Combat Manager references")]
    public CombatManager CombatManager;*/

    [SerializeField] private Spirithar _playerSpirithar;
    [SerializeField] private GameObject _playerSpiritharGO;
    [SerializeField] private int _spiritharIndex;
    private TextMeshProUGUI _textSpiritharName;

    private void Awake() {
        _textSpiritharName = Button.GetComponentInChildren<TextMeshProUGUI>();
        _textSpiritharName.text = "Pruebaaa";
        //SetUpTextAbilityButton("Hooolaaa");
    }

    public void SetUpTextSpiritharNameButton(Spirithar spirithar, int indexOfTeam) {
        if (spirithar != null) {
            _playerSpirithar = spirithar;
            _spiritharIndex = indexOfTeam;
            _textSpiritharName.text = _playerSpirithar.spiritharName;
        } else
            _textSpiritharName.text = "No Spirithar";
        
    }

    public void OnChangeSpirithar(){
        OnSpiritharClickToChange?.Invoke(_playerSpirithar.gameObject, _spiritharIndex);


    }

    /*public void AbilityMove() {

        if (CombatManager.Instance.State != BattleCaptureState.PLAYERTURN || _playerSpirithar.PerformingMove)
            return;
        // No sé si hacerlo aquí o en CombatManager y si hacerlo o intentar con un Action
        // Pero pasarle a los button una función en el OnClick que sea dinámica. Según el Move que se le pase (lo tenemos en _spiritharMove) hacer el ataque la subida de defensa o subida de poder
        _playerSpirithar.PerformMove(_spiritharMove, _enemySpirithar); // On click del button hace el performove del spirithar (algo así hay que darle alguna vuelta)

        // Luego manejar los estados de los turnos ESO SI QUE SI EN EL COMBAT MANAGER. Invocar algo para que combat manager maneje los turnos y alomejor compruebe las muertes y demas
    }*/
}
