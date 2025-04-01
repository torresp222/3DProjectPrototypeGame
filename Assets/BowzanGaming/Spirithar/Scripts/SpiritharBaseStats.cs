using UnityEngine;

[CreateAssetMenu(fileName = "NewSpiritharBaseStats", menuName = "Spirithar/BaseStats")]
public class SpiritharBaseStats : ScriptableObject {
    public float baseHealth;
    public float baseAttack;
    public float baseDefense;
    public int baseSpeed;
    // Puedes agregar otros atributos como velocidad, precisión, etc.
}