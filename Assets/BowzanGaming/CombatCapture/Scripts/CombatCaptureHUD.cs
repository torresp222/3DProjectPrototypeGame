using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatCaptureHUD : MonoBehaviour
{
    [Header("Ui Combat references")]
    public TextMeshProUGUI SpNameText;
    public TextMeshProUGUI SpLvlText;
    public Slider SpHPSlider;

    // Set the names, Lvl, HP of the Spirithar in the ui combat capture
    public void SetUpCaptureCombatHUD(Spirithar spirithar) {
        SpNameText.text = spirithar.spiritharName;
        SpLvlText.text = "Lvl " + spirithar.Lvl.ToString();
        SpHPSlider.maxValue = spirithar.maxHealth;
        SpHPSlider.value = spirithar.currentHealth;
       // SetHP(spirithar);
    }

    public void SetHP(float currentHealth) {
        SpHPSlider.value = currentHealth;
    }
}
