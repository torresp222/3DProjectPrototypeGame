using UnityEngine;

public enum MoveType {
    Attack,
    Defense,
    Boost
}

[CreateAssetMenu(fileName = "NewSpiritharMove", menuName = "Spirithar/Move")]
public class SpiritharMove : ScriptableObject {
    public string moveName;
    public MoveType moveType;
    public int power;      // Daño para ataques o valor de efecto para otros movimientos
    public float cooldown; // Tiempo de recarga del movimiento
}
