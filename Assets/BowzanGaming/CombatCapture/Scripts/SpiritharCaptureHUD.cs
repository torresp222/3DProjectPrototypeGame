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

    public Spirithar PlayerSpirithar {  get { return _playerSpirithar; } }

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

    public void DisableButton() {
        Debug.Log($"Deshabilitando button de {_spiritharIndex} - {_textSpiritharName.text}");
        Button.interactable = false;
    }

    public void EnableButton() {
        Debug.Log($"HABILITANDO button de {_spiritharIndex} - {_textSpiritharName.text}");
        Button.interactable = true;
    }
}
