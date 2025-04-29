using TMPro;
using UnityEngine;

public class UISpiritharSlot : MonoBehaviour {
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthPercentageText;

    public void UpdateSlot(string spiritharName, float currentHealth, float maxHealth) {
        nameText.text = spiritharName;

        float percentage = Mathf.Max((currentHealth / maxHealth) * 100f, 0f);
        healthPercentageText.text = $"{percentage:F0}%";
    }

    public void ClearSlot() {
        nameText.text = "Empty";
        healthPercentageText.text = "0%";
    }
}